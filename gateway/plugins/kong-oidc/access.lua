local openidc = require("kong.plugins.kong-oidc.lib.resty.openidc")
local cjson = require("cjson.safe")

local _M = {}

-- paths that must NOT trigger OIDC
local function is_ignored_path(path)
  if not path then return false end

  -- static assets
  if path:match("^/assets/") then return true end
  if path:match("%.js$") then return true end
  if path:match("%.css$") then return true end
  if path:match("%.svg$") then return true end
  if path:match("%.ico$") then return true end
  if path:match("%.map$") then return true end

  return false
end

function _M.execute(conf)
  local path = kong.request.get_path()

  -- ✅ BYPASS OIDC FOR STATIC FILES
  if is_ignored_path(path) then
    return
  end

  local opts = {
    client_id = conf.client_id,
    client_secret = conf.client_secret,
    discovery = conf.discovery,
    redirect_uri = conf.redirect_uri,
    scope = conf.scope or "openid profile email",
    issuer = conf.issuer,

    session_secret = conf.session_secret or "some_long_random_secret",
    cookie = {
      name = "oidc_session",
      secure = false, -- true in prod
      http_only = true,
      path = "/",
      samesite = "Lax",
    },
  }

  local res, err = openidc.authenticate(opts)

  if err then
    return kong.response.exit(401, {
      message = "Authentication failed: " .. err
    })
  end

  -- pass user info upstream
  kong.service.request.set_header("X-Userinfo", cjson.encode(res))
end

return _M
