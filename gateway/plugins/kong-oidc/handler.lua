local http = require("resty.http")
local cjson = require("cjson.safe")
local kong = kong

local plugin = {
  PRIORITY = 1000,
  VERSION = "1.0.6",
}

local function build_auth_url(conf, state)
  return string.format(
    "%s/protocol/openid-connect/auth?client_id=%s&response_type=code&scope=%s&redirect_uri=%s&state=%s",
    conf.issuer,
    ngx.escape_uri(conf.client_id),
    ngx.escape_uri(conf.scope),
    ngx.escape_uri(conf.redirect_uri),
    ngx.escape_uri(state)
  )
end

local function exchange_code_for_token(conf, code)
  local httpc = http.new()
  local res, err = httpc:request_uri(conf.issuer .. "/protocol/openid-connect/token", {
    method = "POST",
    body = ngx.encode_args({
      grant_type = "authorization_code",
      code = code,
      redirect_uri = conf.redirect_uri,
      client_id = conf.client_id,
      client_secret = conf.client_secret,
    }),
    headers = { ["Content-Type"] = "application/x-www-form-urlencoded" },
    ssl_verify = conf.ssl_verify,
  })

  if not res then
    kong.log.err("Token exchange failed: ", err)
    return nil, err
  end

  local body = cjson.decode(res.body)
  if not body or not body.access_token then
    kong.log.err("Invalid token response: ", res.body)
    return nil, "Invalid token response"
  end

  return body
end

function plugin:access(conf)
  local args = kong.request.get_query()
  local state_cookie_name = "oidc_state"

  -- Handle Keycloak callback with code
  if args.code then
    local state_cookie = ngx.var["cookie_" .. state_cookie_name]
    if not state_cookie or state_cookie ~= args.state then
      return kong.response.exit(400, { message = "Invalid state parameter" })
    end

    local token_data, err = exchange_code_for_token(conf, args.code)
    if not token_data then
      return kong.response.exit(401, { message = "Token exchange failed", error = err })
    end

    -- Pass token to Angular via header
    kong.response.set_header("X-Access-Token", token_data.access_token)

    -- Clear state cookie
    ngx.header["Set-Cookie"] = state_cookie_name .. "=; Path=/; Max-Age=0"

    -- Redirect to original path
    local redirect_path = "/"
    local state_parts = ngx.decode_base64(args.state or "")
    if state_parts then
      local sep_index = state_parts:find("-")
      if sep_index then
        redirect_path = state_parts:sub(sep_index + 1)
      end
    end

    kong.response.set_header("Location", redirect_path)
    return kong.response.exit(302)
  end

  -- Redirect unauthenticated users
  local client_auth_header = kong.request.get_header("Authorization")
  if not client_auth_header then
    local state = ngx.encode_base64(ngx.time() .. "-" .. kong.request.get_path())
    ngx.header["Set-Cookie"] = state_cookie_name .. "=" .. state .. "; Path=/; HttpOnly"
    local auth_url = build_auth_url(conf, state)
    kong.response.set_header("Location", auth_url)
    return kong.response.exit(302)
  end

  -- Forward client Authorization header to backend
  kong.service.request.set_header("Authorization", client_auth_header)
end

return plugin
