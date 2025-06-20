using approvals.application.DTOs.StageDefinition;

namespace approvals.application.DTOs.ApplicationType
{
    public class GetAppTemplateDto 
    {
        public Guid TemplateId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public List<GetStageDefinitionDto> StageDefinitions { get; set; } = new();
    }
}
