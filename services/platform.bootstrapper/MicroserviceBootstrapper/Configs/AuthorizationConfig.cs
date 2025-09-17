namespace MicroserviceBootstrapper.Configs
{
    public class AuthorizationConfig
    {
        public string? BaseUrl { get; set; }
        public string? ApiToken { get; set; }
        public string StoreName { get; init; } = "default"; // sensible fallback
    }
}
