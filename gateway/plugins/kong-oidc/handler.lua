local http = require("resty.http")
local cjson = require("cjson.safe")
local kong = kong

local plugin = {
  PRIORITY = 1000,   -- high priority
  VERSION = "1.0.2",
}

-- Use a shared dictionary for caching tokens
local token_cache = ngx.shared.kong_oidc_cache

-- Build Keycloak login URL
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

-- Exchange code for tokens
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

-- Fetch user info
local function fetch_userinfo(conf, token)
  local httpc = http.new()
  local res, err = httpc:request_uri(conf.issuer .. "/protocol/openid-connect/userinfo", {
    method = "GET",
    headers = { ["Authorization"] = "Bearer " .. token },
    ssl_verify = conf.ssl_verify,
  })

  if not res then
    kong.log.err("Userinfo fetch failed: ", err)
    return nil, err
  end

  return cjson.decode(res.body)
end

-- Main plugin access
function plugin:access(conf)
  local args = kong.request.get_query()
  local state_cookie_name = "oidc_state"
  local session_cookie_name = "oidc_session"

  -- Retrieve session ID from cookie
  local session_id = ngx.var["cookie_" .. session_cookie_name]

  -- 1️⃣ Handle callback from Keycloak
  if args.code then
    local state_cookie = ngx.var["cookie_" .. state_cookie_name]
    if not state_cookie or state_cookie ~= args.state then
      return kong.response.exit(400, { message = "Invalid state parameter" })
    end

    -- Exchange code for tokens
    local token_data, err = exchange_code_for_token(conf, args.code)
    if not token_data then
      return kong.response.exit(401, { message = "Token exchange failed", error = err })
    end

    -- Fetch user info
    local userinfo, uerr = fetch_userinfo(conf, token_data.access_token)
    if not userinfo then
      return kong.response.exit(401, { message = "Userinfo fetch failed", error = uerr })
    end

    -- Generate session ID
    session_id = ngx.encode_base64(token_data.access_token .. "-" .. ngx.time())

    -- Cache tokens in Kong (expire with token lifetime)
    local expire = tonumber(token_data.expires_in) or 3600
    token_cache:set(session_id .. "_access", token_data.access_token, expire)
    token_cache:set(session_id .. "_refresh", token_data.refresh_token, expire * 2)
    token_cache:set(session_id .. "_id", token_data.id_token, expire)

    -- Set session cookie
    ngx.header["Set-Cookie"] = session_cookie_name .. "=" .. session_id .. "; Path=/; HttpOnly"

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

  -- 2️⃣ Redirect unauthenticated users to Keycloak
  if not session_id then
    local state = ngx.encode_base64(ngx.time() .. "-" .. kong.request.get_path())
    ngx.header["Set-Cookie"] = state_cookie_name .. "=" .. state .. "; Path=/; HttpOnly"
    local auth_url = build_auth_url(conf, state)
    kong.response.set_header("Location", auth_url)
    return kong.response.exit(302)
  end

  -- 3️⃣ Authenticated users → forward cached token to upstream
  local access_token = token_cache:get(session_id .. "_access")
  if access_token then
    kong.service.request.set_header("Authorization", "Bearer " .. access_token)
  else
    -- Optional: redirect to Keycloak if cached token expired
    local state = ngx.encode_base64(ngx.time() .. "-" .. kong.request.get_path())
    ngx.header["Set-Cookie"] = state_cookie_name .. "=" .. state .. "; Path=/; HttpOnly"
    local auth_url = build_auth_url(conf, state)
    kong.response.set_header("Location", auth_url)
    return kong.response.exit(302)
  end

  -- Optional: set user info header for upstream
  local userinfo_json = token_cache:get(session_id .. "_id")
  if userinfo_json then
    kong.service.request.set_header("X-User", userinfo_json)
  end
end

return plugin
