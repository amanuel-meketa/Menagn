using approvals.domain.Entities;
using approvals.shared.Repositories;

namespace approvals.application.Interfaces.Repository
{
    public interface IStageInstanceRepository : IGenericRepository<StageInstance> 
    {
        Task<StageInstance?> GetByIdAsync(Guid stageInstanceId);
        Task UpdateAsync(StageInstance stageInstance);
    }
}
