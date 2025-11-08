using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Utils;

namespace MicroserviceBootstrapper.Initializers
{
    public class ApiGatewayInitializer : BaseServiceInitializer
    {
        private readonly ApiGatewayConfig _gatewayConfig;
        private readonly AuthenticationConfig _authConfig;
        private readonly HttpClient _httpClient;

        public ApiGatewayInitializer(IOptions<ApiGatewayConfig> gatewayConfig, IOptions<AuthenticationConfig> authOptions, Logger logger): base(logger)
        {
            _gatewayConfig = gatewayConfig.Value;
            _authConfig = authOptions.Value;
            _httpClient = new HttpClient { BaseAddress = new Uri(_gatewayConfig.AdminUrl) };
        }

        public override async Task InitializeAsync()
        {
            _logger.Info("🚀 Initializing Kong API Gateway configuration...");

            foreach (var svc in _gatewayConfig.Services)
            {
                await EnsureServiceAndRouteAsync(svc.Name, svc.Url, svc.Path);
            }

            await EnsureGlobalOidcPluginAsync();

            _logger.Success("✅ Kong API Gateway initialized successfully.");
        }

        private async Task EnsureServiceAndRouteAsync(string serviceName, string serviceUrl, string routePath)
        {
            // --- Check if service already exists ---
            var serviceExists = await ResourceExistsAsync($"/services/{serviceName}");
            if (!serviceExists)
            {
                _logger.Info($"🛠 Creating service: {serviceName}");
                var serviceResponse = await _httpClient.PostAsJsonAsync("/services", new
                {
                    name = serviceName,
                    url = serviceUrl
                });

                if (!serviceResponse.IsSuccessStatusCode)
                {
                    var err = await serviceResponse.Content.ReadAsStringAsync();
                    _logger.Error($"❌ Failed to create service '{serviceName}': {err}");
                    return;
                }
            }
            else
            {
                _logger.Info($"ℹ️ Service '{serviceName}' already exists. Skipping.");
            }

            // --- Check if route already exists ---
            var routeExists = await ResourceExistsAsync($"/routes/{serviceName}-route");
            if (!routeExists)
            {
                _logger.Info($"🛠 Creating route for {serviceName} → {routePath}");
                var routeResponse = await _httpClient.PostAsJsonAsync($"/services/{serviceName}/routes", new
                {
                    name = $"{serviceName}-route",
                    paths = new[] { routePath },
                    strip_path = false
                });

                if (!routeResponse.IsSuccessStatusCode)
                {
                    var err = await routeResponse.Content.ReadAsStringAsync();
                    _logger.Error($"❌ Failed to create route for '{serviceName}': {err}");
                }
            }
            else
            {
                _logger.Info($"ℹ️ Route '{serviceName}-route' already exists. Skipping.");
            }
        }

        private async Task EnsureGlobalOidcPluginAsync()
        {
            // --- Check if global OIDC plugin already exists ---
            var pluginsResponse = await _httpClient.GetAsync("/plugins");
            if (pluginsResponse.IsSuccessStatusCode)
            {
                var json = await pluginsResponse.Content.ReadAsStringAsync();
                if (json.Contains("\"kong-oidc\""))
                {
                    _logger.Info("🔒 Global OIDC plugin already configured. Skipping.");
                    return;
                }
            }

            _logger.Info("🛠 Configuring global OIDC plugin...");

            var pluginResponse = await _httpClient.PostAsJsonAsync("/plugins", new
            {
                name = "kong-oidc",
                enabled = true,
                config = new
                {
                    client_id = _authConfig.ClientId,
                    client_secret = _authConfig.ClientSecret,
                    issuer = $"{_gatewayConfig.Issuer}/realms/{_authConfig.Realm}",
                    redirect_uri = _authConfig.RedirectUri,
                    scope = "openid profile",
                    unauth_action = "redirect"
                }
            });

            if (pluginResponse.IsSuccessStatusCode)
                _logger.Success("✅ Global OIDC plugin configured successfully.");
            else
            {
                var err = await pluginResponse.Content.ReadAsStringAsync();
                _logger.Error($"❌ Failed to configure OIDC plugin: {err}");
            }
        }

        private async Task<bool> ResourceExistsAsync(string path)
        {
            try
            {
                var response = await _httpClient.GetAsync(path);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
