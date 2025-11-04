return {
  name = "kong-oidc",
  fields = {
    {
      config = {
        type = "record",
        fields = {
          -- 🔹 Required core OIDC parameters
          { issuer = { type = "string", required = true } },
          { client_id = { type = "string", required = true } },
          { client_secret = { type = "string", required = true } },

          -- 🔹 Authorization Code Flow endpoints
          { redirect_uri = { type = "string", required = true } },
          { authorization_endpoint = { type = "string", required = false, default = "" } },
          { token_endpoint = { type = "string", required = false, default = "" } },
          { userinfo_endpoint = { type = "string", required = false, default = "" } },

          -- 🔹 Standard OIDC settings
          { scope = { type = "string", default = "openid profile email" } },
          { response_type = { type = "string", default = "code" } },
          { grant_type = { type = "string", default = "authorization_code" } },

          -- 🔹 Session and logout handling
          { logout_path = { type = "string", default = "/logout" } },
          { unauth_action = { type = "string", default = "redirect" } },
          { session_cookie_name = { type = "string", default = "oidc_session" } },
          { session_timeout = { type = "number", default = 3600 } },

          -- 🔹 Security and debugging
          { ssl_verify = { type = "boolean", default = false } },
          { debug = { type = "boolean", default = false } },
        },
      },
    },
  },
}
