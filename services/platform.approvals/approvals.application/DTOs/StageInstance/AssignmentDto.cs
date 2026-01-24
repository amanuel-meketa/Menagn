namespace approvals.application.DTOs.StageInstance
{
    public class AssignmentDto
    {
        public Guid StageInstanceId { get; set; }
        public Guid ApproverId { get; set; }
    }
}
