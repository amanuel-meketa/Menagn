return {
  name = "kong-oidc",
  fields = {
    {
      config = {
        type = "record",
        fields = {
          { issuer = { type = "string", required = true, default = "http://host.docker.internal:8180/realms/Menagn" } },
          { client_id = { type = "string", required = true, default = "menagn" } },
          { client_secret = { type = "string", required = true, default = "ShxxjZKYS9JMLwRyi0fBanG0InzbnbhY" } },
          { redirect_uri = { type = "string", required = true, default = "http://localhost:8000/setTokenFromKong" } },
          { authorization_endpoint = { type = "string", default = "/protocol/openid-connect/auth" } },
          { token_endpoint = { type = "string", default = "/protocol/openid-connect/token" } },
          { userinfo_endpoint = { type = "string", default = "/protocol/openid-connect/userinfo" } },
          { scope = { type = "string", default = "openid profile" } },
          { response_type = { type = "string", default = "code" } },
          { grant_type = { type = "string", default = "authorization_code" } },
          { logout_path = { type = "string", default = "/logout" } },
          { unauth_action = { type = "string", default = "redirect" } },
          { session_cookie_name = { type = "string", default = "oidc_session" } },
          { session_timeout = { type = "number", default = 3600 } },
          { ssl_verify = { type = "boolean", default = false } },
          { debug = { type = "boolean", default = false } },
          { enabled = { type = "boolean", required = true, default = true } },
        },
      },
    },
  },
}
