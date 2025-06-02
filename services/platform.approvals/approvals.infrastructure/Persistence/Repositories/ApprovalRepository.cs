using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.infrastructure.Persistence.Repositories.Base;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class ApprovalRepository : GenericRepository<ApplicationType>, IApprovalRepository
    {
        public ApprovalRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
