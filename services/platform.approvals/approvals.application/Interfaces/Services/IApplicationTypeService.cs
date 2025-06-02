using approvals.application.DTOs.ApplicationType;

public interface IApplicationTypeService
{
    Task<Guid> CreateApplicationTypeAsync(CreateApplicationTypeDto dto);
    Task<GetApplicationTypeDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GetApplicationTypeDto>> GetAllAsync();
    Task<Guid> UpdateAsync(Guid id, UpdateApplicationTypeDto dto);
    Task<bool> DeleteAsync(Guid id);
}
