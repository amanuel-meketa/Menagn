return {
  name = "kong-oidc",
  fields = {
    {
      config = {
        type = "record",
        fields = {
          { issuer = { type = "string", required = true } },
          { client_id = { type = "string", required = true } },
          { client_secret = { type = "string", required = true } },

          -- Provide valid defaults (length ≥ 1)
          { redirect_uri = { type = "string", required = true, default = "http://localhost:8000" } },
          { authorization_endpoint = { type = "string", required = false, default = "/protocol/openid-connect/auth" } },
		  { token_endpoint         = { type = "string", required = false, default = "/protocol/openid-connect/token" } },
		  { userinfo_endpoint      = { type = "string", required = false, default = "/protocol/openid-connect/userinfo" } },

          { scope = { type = "string", default = "openid" } },
          { response_type = { type = "string", default = "code" } },
          { grant_type = { type = "string", default = "authorization_code" } },

          { logout_path = { type = "string", default = "/logout" } },
          { unauth_action = { type = "string", default = "redirect" } },
          { session_cookie_name = { type = "string", default = "oidc_session" } },
          { session_timeout = { type = "number", default = 3600 } },

          { ssl_verify = { type = "boolean", default = false } },
          { debug = { type = "boolean", default = false } },
        },
      },
    },
  },
}
