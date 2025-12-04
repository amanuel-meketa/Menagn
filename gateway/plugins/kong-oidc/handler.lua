-- Main plugin handler
local openidc = require("kong.plugins.kong-oidc.lib.resty.openidc")
local access = require("kong.plugins.kong-oidc.access") 

local KongOIDC = {
  PRIORITY = 1000, -- execution priority
  VERSION = "0.1",
}

function KongOIDC:access(plugin_conf)
  access.execute(plugin_conf)
end

return KongOIDC
