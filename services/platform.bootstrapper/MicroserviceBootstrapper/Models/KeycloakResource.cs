
namespace MicroserviceBootstrapper.Models
{
    public class KeycloakResource
    {
        public string Name { get; set; }
        public List<string> Scopes { get; set; } = new();
    }
}
