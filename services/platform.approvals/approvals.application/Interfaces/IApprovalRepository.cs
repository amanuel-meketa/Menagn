using approvals.application.DTOs;

namespace approvals.application.Interfaces
{
    public interface IApprovalRepository
    {
        Task<IEnumerable<ApprovalDto>> GetAllAsync();
        Task<ApprovalDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(ApprovalDto approval);
        Task<bool> UpdateAsync(ApprovalDto approval);
        Task<bool> DeleteAsync(Guid id);
    }
}
