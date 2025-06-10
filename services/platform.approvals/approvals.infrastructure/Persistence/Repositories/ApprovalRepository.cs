using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.infrastructure.Persistence;
using approvals.infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

public class ApprovalRepository : GenericRepository<ApprovalTemplate>, IApprovalRepository
{
    private readonly AppDbContext _dbContext;

    public ApprovalRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<ApprovalTemplate?> GetByIdAsync(Guid id)
    {
        return await _dbContext.ApprovalTemplates.Include(t => t.StageDefinitions).FirstOrDefaultAsync(t => t.TemplateId == id);
    }
}
