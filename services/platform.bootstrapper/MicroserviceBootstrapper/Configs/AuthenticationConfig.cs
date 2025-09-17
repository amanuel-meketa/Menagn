namespace MicroserviceBootstrapper.Configs;

public sealed record AuthenticationConfig
{
    public string? BaseUrl { get; init; }
    public string? Realm { get; init; }
    public string? AdminUser { get; init; }
    public string? AdminPassword { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
}
