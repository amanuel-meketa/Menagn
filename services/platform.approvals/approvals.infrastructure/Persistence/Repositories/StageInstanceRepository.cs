using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.domain.Enums;
using approvals.infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class StageInstanceRepository(AppDbContext dbContext) : GenericRepository<StageInstance>(dbContext), IStageInstanceRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<StageInstance?> GetByIdAsync(Guid stageInstanceId)
        {
            return await _dbContext.StageInstances.FirstOrDefaultAsync(si => si.StageInstanceId == stageInstanceId);
        }

        public async Task UpdateAsync(StageInstance stageInstance)
        {
            _dbContext.StageInstances.Update(stageInstance);
        }

        public async Task<List<StageInstance>> GetActiveTasksForUserAsync(Guid userId)
        {
            return await _dbContext.StageInstances.Include(s => s.StageDefinition)
                .Where(s => s.AssignedApproverId == userId && s.Status == StageInstanceStatus.Active).OrderBy(s => s.StartedAt).ToListAsync();
        }
    }
}

