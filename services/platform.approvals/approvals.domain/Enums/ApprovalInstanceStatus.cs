namespace approvals.domain.Enums
{
    public enum ApprovalInstanceStatus
    {
        Draft = 0,       // Created but not submitted
        InProgress = 1,  // At least one stage is Active
        Approved = 2,    // All stages approved
        Rejected = 3,    // Any stage rejected
        Cancelled = 4    // Cancelled by requester/admin
    }
}
