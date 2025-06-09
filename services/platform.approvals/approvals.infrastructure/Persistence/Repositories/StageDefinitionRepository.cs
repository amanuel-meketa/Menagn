using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.infrastructure.Persistence.Repositories.Base;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class StageDefinitionRepository : GenericRepository<StageDefinition>, IStageDefinitionRepository
    {
        public StageDefinitionRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
