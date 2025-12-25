
namespace security.data.Entities.Events
{
    public sealed class RoleCreatedEvent
    {
        public Guid RoleId { get; init; }
        public string RoleName { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }
}
