using approvals.application.DTOs.StageDefinition;

public interface IApprovalInstanceService
{
    Task<Guid> StartAppInstance(Guid TemplateId, Guid createdBy);
}
