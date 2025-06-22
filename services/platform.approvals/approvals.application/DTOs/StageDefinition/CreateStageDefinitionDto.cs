namespace approvals.application.DTOs.StageDefinition
{
    public class CreateStageDefinitionDto 
    {
        public Guid TemplateId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public string AssignmentType { get; set; } = string.Empty;
        public string AssignmentKey { get; set; } = string.Empty;
    }
}
