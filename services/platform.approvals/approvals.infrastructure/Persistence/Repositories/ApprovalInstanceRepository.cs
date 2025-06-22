using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
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

        public override async Task<ApprovalInstance?> GetByIdAsync(Guid id)
        {
           return  await _dbContext.ApprovalInstances.Include(t => t.StageInstances).FirstOrDefaultAsync(t => t.InstanceId == id);
        }

        public async Task<IEnumerable<ApprovalInstance?>> GetByTemplateIdAsync(Guid templateId)
        {
            return await _dbContext.ApprovalInstances.Where(t => t.TemplateId == templateId).ToListAsync();
        }

        public async Task<ApprovalInstance> CreateApprovalInstanceAsync(Guid templateId, Guid createdBy, List<StageDefinition> stageDefinitions)
        {
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
                        Status = "Pending"
                    }).ToList()
            };

            await _dbContext.ApprovalInstances.AddAsync(instance);

            return instance;
        }
    }
}
