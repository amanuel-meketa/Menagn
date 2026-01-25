namespace approvals.domain.Enums
{
    public enum StageInstanceStatus
    {
        Pending = 0,     // Created but not active
        Active = 1,      // Current stage, approver can act
        Approved = 2,    // Approved by approver
        Rejected = 3,    // Rejected by approver
        Skipped = 4,     // Bypassed by rule/system
        Cancelled = 5    // Cancelled by user/admin
    }
}
