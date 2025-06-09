namespace approvals.application.DTOs.ApprovalInstance
{
    public class ApproveStageRequestDto
    {
        public Guid InstanceId { get; set; }
        public Guid ApproverId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
