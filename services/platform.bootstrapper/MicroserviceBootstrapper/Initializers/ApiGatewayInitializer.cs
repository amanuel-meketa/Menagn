using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Options;

namespace MicroserviceBootstrapper.Initializers
{
    public class ApiGatewayInitializer : BaseServiceInitializer
    {
        private readonly ApiGatewayConfig _config;
        private readonly HttpClient _httpClient;

        public ApiGatewayInitializer(IOptions<ApiGatewayConfig> configOptions, Logger logger): base(logger)
        {
            _config = configOptions.Value;
            _httpClient = new HttpClient { BaseAddress = new Uri(_config.AdminApiUrl) };
        }

        public override async Task InitializeAsync()
        {
            _logger.Info("Initializing Kong global OIDC plugin (kong-oidc)...");

            var pluginPayload = new Dictionary<string, string>
            {
                ["name"] = "kong-oidc",
                ["config.client_id"] = _config.ClientId,
                ["config.client_secret"] = _config.ClientSecret,
                ["config.issuer"] = _config.Issuer,
                ["config.redirect_uri"] = _config.RedirectUri,
                ["config.scope"] = "openid profile",
                ["enabled"] = "true"
            };

            var response = await _httpClient.PostAsync("/plugins", new FormUrlEncodedContent(pluginPayload));

            if (response.IsSuccessStatusCode)
            {
                _logger.Success("✅ kong-oidc plugin created globally in Kong Gateway.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.Error($"❌ Failed to create kong-oidc plugin: {response.StatusCode} - {error}");
            }
        }
    }
}
