using approvals.application.DTOs.StageDefinition;

namespace approvals.application.DTOs.ApprovalInstance
{
    public class CreateApprovaleInstanceDto
    {
        public Guid InstanceId { get; set; } 
        public Guid TemplateId { get; set; }

        public Guid CreatedBy { get; set; }
        public int CurrentStageOrder { get; set; }
        public string? OverallStatus { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public List<GetStageInstanceDto> StageInstances { get; set; } = new();
    }
}