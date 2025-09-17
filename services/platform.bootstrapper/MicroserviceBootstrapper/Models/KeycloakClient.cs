
namespace MicroserviceBootstrapper.Models
{
    public class KeycloakClient
    {
        required
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Secret { get; set; }
        public List<string> RedirectUris { get; set; } = new();
    }
}
