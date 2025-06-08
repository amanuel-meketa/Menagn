using approvals.domain.Entities.Common;

namespace approvals.domain.Entities
{
    public class ApprovalInstance : EntityBase
    {
        public Guid TemplateId { get; set; }
        public int CurrentStageOrder { get; set; }
        public string OverallStatus { get; set; } = "Pending";
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApprovalTemplate Template { get; set; } = null!;
        public ICollection<StageInstance> StageInstances { get; set; } = new List<StageInstance>();
    }
}
