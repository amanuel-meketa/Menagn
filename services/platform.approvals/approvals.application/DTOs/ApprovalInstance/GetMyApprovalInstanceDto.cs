
namespace approvals.application.DTOs.ApprovalInstance
{
    public class GetMyApprovalInstanceDto
    {
        public Guid InstanceId { get; set; }
        public Guid TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public int CurrentStageOrder { get; set; }
        public int TotalStages { get; set; }
        public string? OverallStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
