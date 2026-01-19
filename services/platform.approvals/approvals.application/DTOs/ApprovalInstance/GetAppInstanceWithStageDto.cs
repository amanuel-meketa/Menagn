using approvals.domain.Entities;

namespace approvals.application.DTOs.ApprovalInstance
{
    public class GetAppInstanceWithStageDto 
    {
        public Guid InstanceId { get; set; }
        public string? TemplateName { get; set; }
        public int CurrentStageOrder { get; set; }
        public string? OverallStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public UserInfoDto? CreatedBy { get; set; }
        public Guid TemplateId { get; set; }
        public ApprovalTemplate? Template { get; set; }
        public List<StageInstance>? StageInstances { get; set; }
    }
}
