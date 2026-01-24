using System.ComponentModel.DataAnnotations;

namespace approvals.domain.Entities
{
    public class StageInstance
    {
        [Key]
        public Guid StageInstanceId { get; set; } = Guid.NewGuid();

        public Guid ApprovalInstanceId { get; set; }
        public ApprovalInstance ApprovalInstance { get; set; } = null!;

        public Guid StageDefId { get; set; }
        public StageDefinition StageDefinition { get; set; } = null!;

        public string StageName { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Comments { get; set; }
        public Guid? AssignedApproverId { get; set; }
        public Guid? ApprovedBy { get; set; }

        public void Activate()
        {
            Status = "InProgress";
            StartedAt = DateTime.UtcNow;
        }

        public void Complete(string decision, Guid approverId, string comment)
        {
            Status = decision;
            ApprovedBy = approverId;
            Comments = comment;
            CompletedAt = DateTime.UtcNow;
        }

        public void AssignApprover(Guid approverId)
        {
            AssignedApproverId = approverId;
        }

        public void UnassignApprover()
        {
            AssignedApproverId = null;
        }
    }
}
