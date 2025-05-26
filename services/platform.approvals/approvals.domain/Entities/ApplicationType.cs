using approvals.domain.Entities.Common;

namespace approvals.domain.Entities
{
    public class ApplicationType : ApplicationBase
    {
        public Guid? FormDefinitionId { get; set; }
        public Guid? WorkflowDefinitionId { get; set; }

        public FormDefinition? FormDefinition { get; set; }
        public ApplicationFlowDefinition? WorkflowDefinition { get; set; }
    }
}
