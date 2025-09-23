
namespace authorization.data.Models
{
    public class RoleResourceAssignment
    {
        public required string Role { get; set; }
        public required string Resource { get; set; } 
        public required IEnumerable<string> Scopes { get; set; }
    }
}
