using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
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

    }
}



