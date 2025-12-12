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
          { discovery = { type = "string", required = true } },
          { issuer = { type = "string", required = false } },
          { redirect_uri = { type = "string", required = true } },
          { scope = { type = "string", default = "openid" } },
          { unauth_action = { type = "string", default = "deny" } },
        },
      },
    },
  },
}
