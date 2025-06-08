using approvals.domain.Entities.Common;

namespace approvals.domain.Entities
{
    public class ApprovalTemplate : Application
    {
        public ICollection<StageDefinition> StageDefinitions { get; set; } = new List<StageDefinition>();
        public ICollection<ApprovalInstance> ApprovalInstances { get; set; } = new List<ApprovalInstance>();
    }
}
