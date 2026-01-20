
namespace approvals.application.DTOs.ApprovalInstance
{
    public class GetMyApprovalInstanceDto
    {
        public Guid InstanceId { get; set; }
        public int CurrentStageOrder { get; set; }
        public int AllStages { get; set; }
        public string? OverallStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public UserInfoDto? CreatedBy { get; set; }
        public AppTemplateDto? Template { get; set; }
    }
}
