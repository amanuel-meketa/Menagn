using approvals.application.DTOs;
using approvals.application.DTOs.ApplicationType;

namespace approvals.application.Interfaces
{
    public interface IApprovalRepository
    {
        Task<IEnumerable<GetApplicationTypeDto>> GetAllAsync();
        Task<GetApplicationTypeDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateApplicationTypeDto approval);
        Task<bool> UpdateAsync(UpdateApplicationTypeDto approval);
        Task<bool> DeleteAsync(Guid id);
    }
}
