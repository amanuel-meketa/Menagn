local kong = kong
local plugin = {
  PRIORITY = 1000,    -- run order, higher = first
  VERSION = "1.0.0",
}

-- Access phase: main plugin logic
function plugin:access(conf)
  kong.log.debug("Running kong-oidc plugin for request: ", kong.request.get_path())

  -- Example: Check Authorization header
  local token = kong.request.get_header("Authorization")
  if not token then
    return kong.response.exit(401, { message = "Missing Authorization header" })
  end

  -- TODO: Add your OIDC introspection or JWT validation logic here
  -- Example pseudo-code:
  -- local valid = oidc_validate(token, conf.issuer, conf.client_id)
  -- if not valid then
  --    return kong.response.exit(403, { message = "Invalid token" })
  -- end
end

return plugin
