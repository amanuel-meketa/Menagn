
namespace authorization.data.Models
{
    public record userRoleAssignment
    {
        public required string UserId { get; set; }
        public required string Role { get; set; }
    }

}    