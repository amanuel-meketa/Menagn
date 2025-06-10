using approvals.application.DTOs.ApplicationType;
using approvals.application.DTOs.ApprovalInstance;

namespace approvals.application.DTOs.StageDefinition
{
    public class CreateStageInstanceDto
    {

        public Guid ApprovalInstanceId { get; set; }
        public GetApprovalInstanceDto ApprovalInstance { get; set; } = null!;

        public Guid StageDefId { get; set; }
        public List<GetStageDefinitionDto> StageDefinition { get; set; } = null!;

        public string StageName { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public string? Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Comments { get; set; }
        public Guid? ApprovedBy { get; set; }
    }
}
