using approvals.domain.Entities.Common;

namespace approvals.domain.Entities
{
    public class StageInstance : EntityBase
    {
        public Guid InstanceId { get; set; }
        public Guid StageDefId { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AssignedTo { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Comments { get; set; }

        public ApprovalInstance Instance { get; set; } = null!;
        public StageDefinition StageDefinition { get; set; } = null!;
    }
}
