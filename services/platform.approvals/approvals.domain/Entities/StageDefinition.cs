using System.ComponentModel.DataAnnotations;

namespace approvals.domain.Entities
{
    public class StageDefinition
    {
        [Key]
        public Guid StageDefId { get; set; } = Guid.NewGuid();
        public Guid TemplateId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public string AssignmentType { get; set; } = string.Empty;
        public string AssignmentKey { get; set; } = string.Empty;

        public ApprovalTemplate Template { get; set; } = null!;
    }
}
