return {
  name = "kong-oidc",
  fields = {
    { config = {
        type = "record",
        fields = {
          { issuer = { type = "string", required = true } },
          { client_id = { type = "string", required = true } },
          { client_secret = { type = "string", required = true } },
          { redirect_uri = { type = "string", required = true } },
          { logout_path = { type = "string", default = "/logout" } },
          { unauth_action = { type = "string", default = "redirect" } }
        }
    }},
  },
}
