
namespace MicroserviceBootstrapper.Configs
{
    public class ApiGatewayConfig
    {
        public string AdminApiUrl { get; set; } = "http://localhost:8001";
        public string ClientId { get; set; } = "menagn";
        public string ClientSecret { get; set; } = "ShxxjZKYS9JMLwRyi0fBanG0InzbnbhY";
        public string Issuer { get; set; } = "http://host.docker.internal:8180/realms/Menagn";
        public string RedirectUri { get; set; } = "http://localhost:8000";
    }
}
