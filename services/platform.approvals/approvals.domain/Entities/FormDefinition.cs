using approvals.domain.Entities.Common;

namespace approvals.domain.Entities
{
    public class FormDefinition : ApplicationBase
    {   
        public string? SchemaJson { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
