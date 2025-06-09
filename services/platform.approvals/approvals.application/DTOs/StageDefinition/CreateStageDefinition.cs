using approvals.application.DTOs.Common;

namespace approvals.application.DTOs.StageDefinition
{
    public class CreateStageDefinitionDto : AppBaseDto
    {
        public Guid? TemplateId { get; set; }
        public int SequenceOrder { get; set; }
        public string AssignmentType { get; set; } = null!;
        public string AssignmentKey { get; set; } = null!;
    }
}
