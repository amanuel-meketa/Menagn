local http = require("resty.http")
local cjson = require("cjson.safe")

local kong = kong

local plugin = {
  PRIORITY = 1000,   -- runs before most plugins
  VERSION = "1.0.0",
}

-- Utility: build redirect URL
local function build_auth_url(conf, state)
  return string.format(
    "%s/protocol/openid-connect/auth?client_id=%s&response_type=code&scope=%s&redirect_uri=%s&state=%s",
    conf.issuer, conf.client_id, ngx.escape_uri(conf.scope), ngx.escape_uri(conf.redirect_uri), state
  )
end

-- Utility: exchange code for token
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

-- Utility: fetch userinfo
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

  -- 1️⃣ If callback with authorization code
  if args.code then
    local token_data, err = exchange_code_for_token(conf, args.code)
    if not token_data then
      return kong.response.exit(401, { message = "Token exchange failed", error = err })
    end

    local userinfo, uerr = fetch_userinfo(conf, token_data.access_token)
    if not userinfo then
      return kong.response.exit(401, { message = "Userinfo fetch failed", error = uerr })
    end

    -- Optionally store user info in header for upstream services
    kong.service.request.set_header("X-User", userinfo.preferred_username or "unknown")

    -- Redirect user to original URL after login
    return kong.response.exit(302, {
      message = "Authenticated",
      redirect = "/"
    })
  end

  -- 2️⃣ No session or token → redirect to Keycloak login
  local auth_header = kong.request.get_header("Authorization")
  if not auth_header or auth_header == "" then
    local state = ngx.encode_base64(ngx.time() .. "-" .. kong.request.get_path())
    local auth_url = build_auth_url(conf, state)
    kong.response.set_header("Location", auth_url)
    return kong.response.exit(302)
  end

  -- 3️⃣ Otherwise, continue request
  kong.log.debug("User is authenticated")
end

return plugin
