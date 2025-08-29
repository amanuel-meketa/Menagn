using approvals.application.DTOs.ApprovalInstance;
public interface IApprovalInstanceService 
{
    Task<GetApprovalInstanceDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GetApprovalInstanceDto>> GetAllAsync();
    Task<Guid> UpdateAsync(Guid id, UpdateApprovaleInstanceDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid> StartAppInstanceAsync(Guid TemplateId, Guid createdBy);
    Task<IEnumerable<GetApprovalInstanceDto?>> GetByTemplateIdAsync(Guid templateId);
    Task<IEnumerable<GetMyApprovalInstanceDto?>> GetMyAppInstances(Guid userId);
}