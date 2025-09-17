namespace MicroserviceBootstrapper.Models.Keycloak
{
    public class KeycloakPolicy
    {
        public string Name { get; set; }
        public string Type { get; set; } = "role";
        public List<string> Roles { get; set; } = new();
    }
}
