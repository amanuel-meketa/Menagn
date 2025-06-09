using System.ComponentModel.DataAnnotations;

namespace approvals.domain.Entities
{
    public class ApprovalTemplate
    {
        [Key]
        public Guid TemplateId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<StageDefinition?> StageDefinitions { get; set; } = new();
    }
}
