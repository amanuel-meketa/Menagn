local openidc = require("kong.plugins.kong-oidc.lib.resty.openidc")
local cjson = require("cjson.safe")

local KongOIDC = {
  PRIORITY = 1000,  -- higher priority than most plugins
  VERSION = "1.0",
}

function KongOIDC:access(plugin_conf)
  local opts = {
    client_id = plugin_conf.client_id,
    client_secret = plugin_conf.client_secret,
    discovery = plugin_conf.issuer .. "/.well-known/openid-configuration",
    redirect_uri = plugin_conf.redirect_uri,
    scope = plugin_conf.scope or "openid profile email",

    -- SESSION CONFIG
    session_secret = plugin_conf.session_secret,
    cookie = {
      name = "debelo_session",
      secure = plugin_conf.cookie_secure,  -- must be true in prod HTTPS
      http_only = true,
      path = "/",
      samesite = plugin_conf.cookie_samesite or "Lax",
    },

    -- behavior when unauthenticated
    unauth_action = plugin_conf.unauth_action or "deny",
  }

  local res, err = openidc.authenticate(opts)

  if err then
    kong.log.err("OIDC authentication failed: ", err)
    return kong.response.exit(401, { message = "Authentication failed" })
  end

  -- attach user info to upstream
  kong.service.request.set_header("X-Userinfo", cjson.encode(res))
end

return KongOIDC