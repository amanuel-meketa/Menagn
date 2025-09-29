using authorization.data.Models.Enums;

namespace authorization.data.Models
{
    public class AccessAssignment
    {
        public ActorType ActorType { get; set; }      // User or Role
        public string? ActorId { get; set; }          // e.g. "123" or "Developer"
        public string? Relation { get; set; }         // e.g. "approver", "viewer"
        public string? ResourceType { get; set; }     // e.g. "ApprovalTemplate"
        public string? ResourceId { get; set; }       // e.g. "23e5ad76-…"
        public DateTimeOffset? AssignedAt { get; set; }
    }
}
