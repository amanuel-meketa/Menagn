using approvals.domain.Entities;
using approvals.shared.Repositories;

namespace approvals.application.Interfaces.Repository
{
    public interface IApprovalInstanceRepository : IGenericRepository<ApprovalInstance>
    {
        Task<ApprovalInstance> CreateApprovalInstanceAsync(Guid TemplateId, UserInfo createdBy, List<StageDefinition> stageDefinitions);
        Task<IEnumerable<ApprovalInstance?>> GetByTemplateIdAsync(Guid templateId);
        Task<IEnumerable<ApprovalInstance?>> GetMyAppInstances(Guid userId);
        Task UpdateAsync(ApprovalInstance instance);
    }
}