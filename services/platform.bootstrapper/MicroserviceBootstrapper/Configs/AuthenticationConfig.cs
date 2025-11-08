namespace MicroserviceBootstrapper.Configs;

public record AuthenticationConfig
{
    public string? BaseUrl { get; init; }
    public string? Realm { get; init; }
    public string? AdminUser { get; init; }
    public string? AdminPassword { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string? RedirectUri { get; init; }
}
