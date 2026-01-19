using approvals.application.DTOs.ApprovalInstance;
public interface IApprovalInstanceService 
{
    Task<GetAppInstanceDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GetAppInstanceDto>> GetAllAsync();
    Task<Guid> UpdateAsync(Guid id, UpdateApprovaleInstanceDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid> StartAppInstanceAsync(Guid TemplateId, UserInfoDto createdBy);
    Task<IEnumerable<GetAppInstanceDto?>> GetByTemplateIdAsync(Guid templateId);
    Task<IEnumerable<GetMyApprovalInstanceDto?>> GetMyAppInstances(Guid userId);
}