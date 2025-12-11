using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace MicroserviceBootstrapper.Initializers;

public class ApiGatewayInitializer : BaseServiceInitializer
{
    private readonly ApiGatewayConfig _gatewayConfig;
    private readonly HttpClient _httpClient;

    public ApiGatewayInitializer(IOptions<ApiGatewayConfig> gatewayConfig, Logger logger) : base(logger)
    {
        _gatewayConfig = gatewayConfig.Value;
        _httpClient = new HttpClient { BaseAddress = new Uri(_gatewayConfig.AdminUrl) };
    }

    public override async Task InitializeAsync()
    {
        _logger.Info("🚀 Initializing Kong API Gateway configuration...");

        foreach (var svc in _gatewayConfig.Services)
            await EnsureServiceAndRouteAsync(svc.Name, svc.Url, svc.Route);

        await EnsureGlobalOidcPluginAsync();

        _logger.Success("✅ Kong API Gateway initialized successfully.");
    }

    // ----------------------------------------------------
    //  FIXED & CLEAN VERSION OF SERVICE + ROUTE CREATION
    // ----------------------------------------------------
    private async Task EnsureServiceAndRouteAsync(string serviceName, string serviceUrl, string routePath)
    {
        try
        {
            var uri = new Uri(serviceUrl);

            string host = uri.Host;
            int port = uri.Port;
            string protocol = uri.Scheme;

            // -------------------------
            // CREATE SERVICE
            // -------------------------
            var serviceExists = await ResourceExistsAsync($"/services/{serviceName}");

            if (!serviceExists)
            {
                _logger.Info($"🛠 Creating service: {serviceName}");

                var serviceResponse = await _httpClient.PostAsJsonAsync("/services", new
                {
                    name = serviceName,
                    protocol = protocol,
                    host = host,
                    port = port,
                    path = routePath
                });

                if (!serviceResponse.IsSuccessStatusCode)
                {
                    var err = await serviceResponse.Content.ReadAsStringAsync();
                    _logger.Error($"❌ Failed to create service '{serviceName}': {serviceResponse.StatusCode} → {err}");
                    throw new Exception($"Kong service creation failed for {serviceName}");
                }
            }
            else
            {
                _logger.Info($"ℹ️ Service '{serviceName}' already exists. Skipping.");
            }

            // -------------------------
            // CREATE ROUTE
            // -------------------------
            var routeExists = await ResourceExistsAsync($"/routes/{serviceName}");

            if (!routeExists)
            {
                _logger.Info($"🛠 Creating route for {serviceName} → {routePath}");

                var routeResponse = await _httpClient.PostAsJsonAsync($"/services/{serviceName}/routes", new
                {
                    name = serviceName,
                    protocols = new[] { "http", "https" },
                    strip_path = true,
                    paths = new[] { routePath }
                });

                if (!routeResponse.IsSuccessStatusCode)
                {
                    var err = await routeResponse.Content.ReadAsStringAsync();
                    _logger.Error($"❌ Failed to create route '{serviceName}': {routeResponse.StatusCode} → {err}");
                    throw new Exception($"Kong route creation failed for {serviceName}");
                }
            }
            else
            {
                _logger.Info($"ℹ️ Route '{serviceName}' already exists. Skipping.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"🔥 EXCEPTION while creating service/route '{serviceName}': {ex.Message}");
            throw;
        }
    }

    // ----------------------------------------------------
    //              BUILD PLUGIN CONFIG
    // ----------------------------------------------------
    public KongPluginRequest ToKongPluginRequest(KongOidcPluginConfig config)
    {
        var kongConfig = new KongOidcConfig
        {
            client_id = config.ClientId,
            client_secret = config.ClientSecret,
            issuer = config.Issuer,

            authorization_endpoint = config.AuthorizationEndpoint,
            token_endpoint = config.TokenEndpoint,
            userinfo_endpoint = config.UserInfoEndpoint,

            redirect_uri = config.RedirectUri,

            logout_path = config.LogoutPath,
            response_type = config.ResponseType,
            grant_type = config.GrantType,
            scope = config.Scope,

            unauth_action = config.UnauthAction,
            session_timeout = config.SessionTimeout,
            session_cookie_name = config.SessionCookieName,

            ssl_verify = config.SslVerify,
            debug = config.Debug
        };

        return new KongPluginRequest
        {
            name = "kong-oidc",
            enabled = true,
            config = kongConfig
        };
    }


    // ----------------------------------------------------
    //              RESOURCE EXISTS HELPER
    // ----------------------------------------------------
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


    // ----------------------------------------------------
    //            TODO: YOUR OIDC GLOBAL PLUGIN
    // ----------------------------------------------------
    private async Task EnsureGlobalOidcPluginAsync()
    {
        _logger.Info("🔍 Checking for existing OIDC plugin...");

        var pluginsResponse = await _httpClient.GetAsync("/plugins");

        if (pluginsResponse.IsSuccessStatusCode)
        {
            var json = await pluginsResponse.Content.ReadAsStringAsync();
            if (json.Contains("\"name\":\"kong-oidc\""))
            {
                _logger.Info("🔒 Global OIDC plugin already configured. Skipping.");
                return;
            }
        }

        _logger.Info("🛠 Configuring global OIDC plugin...");

        var pluginConfig = ToKongPluginRequest(_gatewayConfig.OIDCPlugin);

        var pluginResponse = await _httpClient.PostAsJsonAsync("/plugins", pluginConfig);

        if (pluginResponse.IsSuccessStatusCode)
        {
            _logger.Success("✅ Global OIDC plugin configured successfully.");
        }
        else
        {
            var error = await pluginResponse.Content.ReadAsStringAsync();
            _logger.Error($"❌ Failed to configure OIDC plugin: {error}");
        }
    }
}
