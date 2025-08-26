using approvals.domain.Entities;
using approvals.shared.Repositories;

namespace approvals.application.Interfaces.Repository
{
    public interface IStageDefinitionRepository : IGenericRepository<StageDefinition> 
    {
        Task<IEnumerable<StageDefinition?>> GetStagesByTempIdAsync(Guid tempId);

        Task<IEnumerable<StageDefinition>> GetAssignedTasksAsync(Guid userId);
    }
}
