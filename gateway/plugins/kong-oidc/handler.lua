local http = require("resty.http")
local cjson = require("cjson.safe")
local kong = kong

local plugin = {
  PRIORITY = 1000,   -- high priority to run before most plugins
  VERSION = "1.0.1",
}

-- Build the Keycloak login URL
local function build_auth_url(conf, state)
  -- URL-encode all parameters properly
  return string.format(
    "%s/protocol/openid-connect/auth?client_id=%s&response_type=code&scope=%s&redirect_uri=%s&state=%s",
    conf.issuer,
    ngx.escape_uri(conf.client_id),
    ngx.escape_uri(conf.scope),
    ngx.escape_uri(conf.redirect_uri),
    ngx.escape_uri(state)
  )
end

-- Exchange authorization code for token
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
    headers = {
      ["Content-Type"] = "application/x-www-form-urlencoded",
    },
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

-- Fetch user info from Keycloak
local function fetch_userinfo(conf, token)
  local httpc = http.new()
  local res, err = httpc:request_uri(conf.issuer .. "/protocol/openid-connect/userinfo", {
    method = "GET",
    headers = {
      ["Authorization"] = "Bearer " .. token,
    },
    ssl_verify = conf.ssl_verify,
  })

  if not res then
    kong.log.err("Userinfo fetch failed: ", err)
    return nil, err
  end

  return cjson.decode(res.body)
end

-- Main plugin access phase
function plugin:access(conf)
  local args = kong.request.get_query()
  local state_cookie_name = "oidc_state"

  -- 1️⃣ Handle callback with authorization code
  if args.code then
    -- Verify state cookie (optional, for CSRF protection)
    local state_cookie = ngx.var["cookie_" .. state_cookie_name]
    if not state_cookie or state_cookie ~= args.state then
      return kong.response.exit(400, { message = "Invalid state parameter" })
    end

    -- Exchange code for token
    local token_data, err = exchange_code_for_token(conf, args.code)
    if not token_data then
      return kong.response.exit(401, { message = "Token exchange failed", error = err })
    end

    -- Fetch user info
    local userinfo, uerr = fetch_userinfo(conf, token_data.access_token)
    if not userinfo then
      return kong.response.exit(401, { message = "Userinfo fetch failed", error = uerr })
    end

    -- Store user info in header for upstream service
    kong.service.request.set_header("X-User", userinfo.preferred_username or "unknown")

    -- Clear state cookie
    ngx.header["Set-Cookie"] = state_cookie_name .. "=; Path=/; Max-Age=0"

    -- Redirect user to original path (decoded from state)
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

  -- 2️⃣ No code → redirect to Keycloak login
  local auth_header = kong.request.get_header("Authorization")
  local session_cookie = ngx.var.cookie_oidc_session

  if not auth_header and not session_cookie then
    -- Generate state for CSRF protection
    local state = ngx.encode_base64(ngx.time() .. "-" .. kong.request.get_path())
    ngx.header["Set-Cookie"] = state_cookie_name .. "=" .. state .. "; Path=/; HttpOnly"
    local auth_url = build_auth_url(conf, state)
    kong.response.set_header("Location", auth_url)
    return kong.response.exit(302)
  end

  -- 3️⃣ Otherwise, user is already authenticated
  kong.log.debug("User is authenticated, continuing request")
end

return plugin
