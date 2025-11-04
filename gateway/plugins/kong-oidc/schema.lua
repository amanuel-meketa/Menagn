return {
  name = "kong-oidc",
  fields = {
    { config = {
        type = "record",
        fields = {
          { issuer = { type = "string", required = true } },
          { client_id = { type = "string", required = true } },
          { client_secret = { type = "string", required = true } },
        }
    }},
  },
}
