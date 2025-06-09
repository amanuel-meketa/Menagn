using approvals.domain.Entities.Common;

namespace approvals.domain.Entities
{
    public class StageDefinition : ApplicationBase
    {
        public Guid? TemplateId { get; set; }
        public int SequenceOrder { get; set; }
        public string AssignmentType { get; set; } = null!;
        public string AssignmentKey { get; set; } = null!;
        public bool ParallelAllowed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApprovalTemplate? Template { get; set; } = null!;
        public ICollection<StageInstance?> StageInstances { get; set; } = new List<StageInstance?>();
    }
}
