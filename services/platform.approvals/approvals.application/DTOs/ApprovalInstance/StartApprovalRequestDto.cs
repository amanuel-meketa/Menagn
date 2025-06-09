namespace approvals.application.DTOs.ApprovalInstance
{
    public class StartApprovalRequestDto
    {
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
    }
}
