using System.ComponentModel.DataAnnotations;

namespace approvals.domain.Entities
{
    public class ApprovalInstance
    {
        [Key]
        public Guid InstanceId { get; set; } = Guid.NewGuid();
        public Guid TemplateId { get; set; }
        public Guid CreatedBy { get; set; }
        public int CurrentStageOrder { get; set; }
        public string OverallStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ApprovalTemplate Template { get; set; } = null!;
        public List<StageInstance> StageInstances { get; set; } = new();
    }
}
