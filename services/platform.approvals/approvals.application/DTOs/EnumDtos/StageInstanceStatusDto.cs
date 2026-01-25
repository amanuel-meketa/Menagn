namespace approvals.application.DTOs.EnumDtos
{
    public enum StageInstanceStatusDto
    {
        Pending,     // Created but not active
        Active,      // Current stage, approver can act
        Approved,    // Approved by approver
        Rejected,    // Rejected by approver
        Skipped,     // Bypassed by rule/system
        Cancelled    // Cancelled by user/admin
    }
}
