using approvals.domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace approvals.domain.Entities
{
    public class ApprovalInstance
    {
        [Key]
        public Guid InstanceId { get; set; } = Guid.NewGuid();
        public int CurrentStageOrder { get; set; }
        public ApprovalInstanceStatus OverallStatus { get; set; } = ApprovalInstanceStatus.InProgress;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public UserInfo CreatedBy { get; set; } = new();
        public string? TemplateName { get; set; }
        public Guid TemplateId { get; set; }
        public ApprovalTemplate Template { get; set; } = null!;
        public List<StageInstance> StageInstances { get; set; } = new();
    }
}
