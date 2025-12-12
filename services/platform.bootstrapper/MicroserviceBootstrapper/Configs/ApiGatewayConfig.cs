using MicroserviceBootstrapper.Configs;

public class ApiGatewayService
{
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Route { get; set; } = null!;
    public bool EnableOIDC { get; set; }
}

public sealed record ApiGatewayConfig
{
    public string AdminUrl { get; init; } = null!;
    public KongOidcPluginConfig OIDCPlugin { get; init; } = new();
    public List<ApiGatewayService> Services { get; init; } = new();
}

public class KongOidcPluginConfig
{
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    public string Discovery { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string RedirectUri { get; init; } = null!;
    public string Scope { get; init; } = "openid profile email";
    public string UnauthAction { get; init; } = "redirect";
}


public class KongPluginRequest
{
    public string name { get; set; } = "kong-oidc";
    public bool enabled { get; set; } = true;
    public KongOidcConfig config { get; set; } = new();
}

public class KongOidcConfig
{
    public string client_id { get; set; }
    public string client_secret { get; set; }
    public string discovery { get; set; }
    public string issuer { get; set; }
    public string redirect_uri { get; set; }
    public string scope { get; set; }
    public string unauth_action { get; set; }
}


