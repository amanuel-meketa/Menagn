using MicroserviceBootstrapper.Configs;

// Each service/route definition
public sealed record ApiGatewayService
{
    public string Name { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string Path { get; init; } = "/";
}

public sealed record ApiGatewayConfig : AuthenticationConfig
{
    public string AdminUrl { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public List<ApiGatewayService> Services { get; init; } = new();
}
