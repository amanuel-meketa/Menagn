using approvals.domain.Entities.Common;

namespace approvals.domain.Entities
{
    public class ApplicationFlowDefinition : ApplicationBase
    {       
        public string? DefinitionJson { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
