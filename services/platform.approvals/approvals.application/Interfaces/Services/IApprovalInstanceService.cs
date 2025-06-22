using approvals.application.DTOs.ApprovalInstance;
using approvals.domain.Entities;
public interface IApprovalInstanceService 
{
    Task<GetApprovalInstanceDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GetApprovalInstanceDto>> GetAllAsync();
    Task<Guid> UpdateAsync(Guid id, UpdateApprovaleInstanceDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid> StartAppInstanceAsync(Guid TemplateId, Guid createdBy);
    Task<IEnumerable<ApprovalInstance?>> GetByTemplateIdAsync(Guid templateId);
}