local plugin = {
  PRIORITY = 1000,
  VERSION = "1.0.0",
}

local oidc = require("resty.openidc")

function plugin:access(conf)
  local opts = {
    discovery = conf.issuer .. "/.well-known/openid-configuration",
    client_id = conf.client_id,
    client_secret = conf.client_secret,
    redirect_uri = conf.redirect_uri,
    scope = "openid profile email",
    ssl_verify = "no", -- or true in production
  }

  local res, err = oidc.authenticate(opts)

  if err then
    kong.log.err("OIDC authentication error: ", err)
    return kong.response.exit(500, { message = "OIDC Auth failed: " .. err })
  end

  kong.service.request.set_header("X-User", res.id_token.sub)
  kong.service.request.set_header("Authorization", "Bearer " .. res.access_token)
end

return plugin
