using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.domain.Enums;
using approvals.infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class ApprovalInstanceRepository : GenericRepository<ApprovalInstance>, IApprovalInstanceRepository
    {
        private readonly AppDbContext _dbContext;

        public ApprovalInstanceRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
         
        public async Task<IEnumerable<ApprovalInstance>> ListAsync()
        {
            return await _dbContext.ApprovalInstances.Include(ai => ai.Template).Include(ai => ai.StageInstances).AsNoTracking().ToListAsync();
        }

        public override async Task<ApprovalInstance?> GetByIdAsync(Guid id)
        {
            return await _dbContext.ApprovalInstances
                .Include(t => t.StageInstances)
                .Include(t => t.Template)
                .FirstOrDefaultAsync(t => t.InstanceId == id);
        }

        public async Task<IEnumerable<ApprovalInstance?>> GetByTemplateIdAsync(Guid templateId)
        {
            return await _dbContext.ApprovalInstances
                .Include(t => t.StageInstances)
                .Include(t => t.Template)
                .Where(t => t.TemplateId == templateId)
                .ToListAsync();
        }

        public async Task<ApprovalInstance> CreateApprovalInstanceAsync( Guid templateId, UserInfo createdBy, List<StageDefinition> stageDefinitions)
        {
            if (createdBy == null)
                throw new ArgumentNullException(nameof(createdBy));

            var instance = new ApprovalInstance
            {
                InstanceId = Guid.NewGuid(),
                TemplateId = templateId,
                CreatedBy = createdBy, 
                CreatedAt = DateTime.UtcNow,
                CurrentStageOrder = 1,
                StageInstances = stageDefinitions
                    .OrderBy(def => def.SequenceOrder)
                    .Select(def => new StageInstance
                    {
                        StageInstanceId = Guid.NewGuid(),
                        StageDefId = def.StageDefId,
                        SequenceOrder = def.SequenceOrder,
                        StageName = def.StageName,
                        Status = StageInstanceStatus.Pending
                    }).ToList()
            };

            await _dbContext.ApprovalInstances.AddAsync(instance);
            return instance;
        }

        public async Task<IEnumerable<ApprovalInstance?>> GetMyAppInstances(Guid userId)
        {
            return await _dbContext.ApprovalInstances
                .Include(x => x.Template)
                .Include(x => x.StageInstances)
                .Where(x => x.CreatedBy.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(ApprovalInstance instance)
        {
            _dbContext.ApprovalInstances.Update(instance);
            await Task.CompletedTask;
        }
    }
}
