using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence.Repositories;

public class StageDefinitionRepository(AppDbContext dbContext) : GenericRepository<StageDefinition>(dbContext), IStageDefinitionRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IEnumerable<StageDefinition?>> GetStagesByTempIdAsync(Guid templateId)
    {
       return await _dbContext.StageDefinitions.AsNoTracking().Where(stage => stage.TemplateId == templateId).ToListAsync();
    }
}
