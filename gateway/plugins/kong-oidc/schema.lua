local typedefs = require "kong.db.schema.typedefs"

return {
  name = "kong-oidc",
  fields = {
    { consumer = typedefs.no_consumer },
    { protocols = typedefs.protocols_http },
    { config = {
        type = "record",
        fields = {
          { client_id = { type = "string", required = true } },
          { client_secret = { type = "string", required = true } },

          -- Core OIDC paths
          { issuer = { type = "string", required = true } },
          { authorization_endpoint = { type = "string", required = true } },
          { token_endpoint = { type = "string", required = true } },
          { userinfo_endpoint = { type = "string", required = true } },

          -- Optional discovery
          { discovery = { type = "string", required = false } },

          -- Redirect
          { redirect_uri = { type = "string", required = true } },

          -- Logout support
          { logout_path = { type = "string", default = "/logout" } },

          -- Behaviour
          { response_type = { type = "string", default = "code" } },
          { grant_type = { type = "string", default = "authorization_code" } },
          { scope = { type = "string", default = "openid profile" } },
          { unauth_action = { type = "string", default = "redirect" } },

          -- Session
          { session_timeout = { type = "integer", default = 3600 } },
          { session_cookie_name = { type = "string", default = "oidc_session" } },

          -- Security
          { ssl_verify = { type = "boolean", default = false } },
          { debug = { type = "boolean", default = false } },
        },
      },
    },
  },
}
