using System.Text.Json.Serialization;

public class ApiGatewayService
{
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Route { get; set; } = null!;
    public bool EnableOIDC { get; set; } = true; // default true if you want OIDC enabled
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

    public string Issuer { get; init; } = null!;
    public string AuthorizationEndpoint { get; init; } = null!;
    public string TokenEndpoint { get; init; } = null!;
    public string UserInfoEndpoint { get; init; } = null!;

    public string RedirectUri { get; init; } = null!;
    public string LogoutPath { get; init; } = "/logout";

    public string ResponseType { get; init; } = "code";
    public string GrantType { get; init; } = "authorization_code";
    public string Scope { get; init; } = "openid profile";

    public string UnauthAction { get; init; } = "redirect";

    public int SessionTimeout { get; init; } = 3600;
    public string SessionCookieName { get; init; } = "oidc_session";

    public bool SslVerify { get; init; } = false;
    public bool Debug { get; init; } = false;
}


public class KongPluginRequest
{
    public string name { get; set; } = "kong-oidc";
    public bool enabled { get; set; } = true;
    public KongOidcConfig config { get; set; } = new();
}

public class KongOidcConfig
{
    [JsonPropertyName("client_id")]
    public string client_id { get; set; }

    [JsonPropertyName("client_secret")]
    public string client_secret { get; set; }

    [JsonPropertyName("redirect_uri")]
    public string redirect_uri { get; set; }

    [JsonPropertyName("issuer")]
    public string issuer { get; set; }

    [JsonPropertyName("authorization_endpoint")]
    public string authorization_endpoint { get; set; }

    [JsonPropertyName("token_endpoint")]
    public string token_endpoint { get; set; }

    [JsonPropertyName("userinfo_endpoint")]
    public string userinfo_endpoint { get; set; }

    [JsonPropertyName("scope")]
    public string scope { get; set; }

    [JsonPropertyName("unauth_action")]
    public string unauth_action { get; set; }

    [JsonPropertyName("logout_path")]
    public string logout_path { get; set; } = "/logout";

    [JsonPropertyName("response_type")]
    public string response_type { get; set; } = "code";

    [JsonPropertyName("grant_type")]
    public string grant_type { get; set; } = "authorization_code";

    [JsonPropertyName("session_timeout")]
    public int session_timeout { get; set; } = 3600;

    [JsonPropertyName("session_cookie_name")]
    public string session_cookie_name { get; set; } = "oidc_session";

    [JsonPropertyName("ssl_verify")]
    public bool ssl_verify { get; set; } = false;

    [JsonPropertyName("debug")]
    public bool debug { get; set; } = false;
}


