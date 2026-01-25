using approvals.application.DTOs.StageInstance;

namespace approvals.application.DTOs.StageDefinition
{
    public class CreateStageDefinitionDto 
    {
        public Guid TemplateId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public AssignmentTypeDto? AssignmentType { get; set; }
        public string AssignmentKey { get; set; } = string.Empty;
    }
}
