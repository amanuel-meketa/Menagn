
namespace MicroserviceBootstrapper.Configs
{
    public class KeycloakConfig
    {
        public string BaseUrl { get; set; }
        public string Realm { get; set; }
        public string AdminUser { get; set; }
        public string AdminPassword { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
