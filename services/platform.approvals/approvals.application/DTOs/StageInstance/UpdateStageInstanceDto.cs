using approvals.application.DTOs.ApprovalInstance;

namespace approvals.application.DTOs.StageDefinition
{
    public class UpdateStageInstanceDto
    {
        public Guid StageInstanceId { get; set; }

        public Guid ApprovalInstanceId { get; set; }
        public GetAppInstanceWithStageDto ApprovalInstance { get; set; } = null!;

        public Guid StageDefId { get; set; }
        public GetStageDefinitionDto StageDefinition { get; set; } = null!;

        public string StageName { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public string Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Comments { get; set; }
        public Guid? ApprovedBy { get; set; }

    }
}
