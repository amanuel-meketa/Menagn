local openidc = require("kong.plugins.kong-oidc.lib.resty.openidc")
local cjson = require("cjson.safe")

local _M = {}

function _M.execute(conf)
  local opts = {
    client_id = conf.client_id,
    client_secret = conf.client_secret,
    discovery = conf.discovery,
    redirect_uri = conf.redirect_uri,
    scope = conf.scope or "openid profile email",
    issuer = conf.issuer,

    -- SESSION CONFIG
    session_secret = conf.session_secret or "some_long_random_secret",
    cookie = {
      name = "oidc_session",
      secure = false,   -- true in prod HTTPS
      http_only = true,
      path = "/",
      samesite = "Lax",
    },
  }

  -- authenticate user (redirect handled automatically)
  local res, err = openidc.authenticate(opts)

  if err then
    return kong.response.exit(401, { message = "Authentication failed: " .. err })
  end

  -- set user info header for upstream
  kong.service.request.set_header("X-Userinfo", cjson.encode(res))
end

return _M
