using approvals.application.DTOs.ApplicationType;

public interface IApprovalTemplateService
{
    Task<Guid> CreateApplicationTypeAsync(CreateApprovalTemplateDto dto);
    Task<GetAppTemplateDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GetAppTemplateDto>> GetAllAsync();
    Task<Guid> UpdateAsync(Guid id, UpdatAppemplateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
