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
          { redirect_uri = { type = "string", required = true, default = "" } },
          { authorization_endpoint = { type = "string", required = false, default = "" } },
          { token_endpoint = { type = "string", required = false, default = "" } },
          { userinfo_endpoint = { type = "string", required = false, default = "" } },

          -- 🔹 Standard OIDC settings
          { scope = { type = "string", required = true, default = "openid" } },
          { response_type = { type = "string", required = true, default = "code" } },
          { grant_type = { type = "string", required = true, default = "authorization_code" } },

          -- 🔹 Session and logout handling
          { logout_path = { type = "string", required = true, default = "/logout" } },
          { unauth_action = { type = "string", required = true, default = "redirect" } },
          { session_cookie_name = { type = "string", required = true, default = "oidc_session" } },
          { session_timeout = { type = "number", required = true, default = 3600 } },

          -- 🔹 Security and debugging
          { ssl_verify = { type = "boolean", required = true, default = true } },
          { debug = { type = "boolean", required = true, default = false } },
        },
      },
    },
  },
}
