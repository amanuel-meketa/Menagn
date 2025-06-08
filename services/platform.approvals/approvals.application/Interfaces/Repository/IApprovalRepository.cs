using approvals.domain.Entities;
using approvals.shared.Repositories;

namespace approvals.application.Interfaces.Repository
{
    public interface IApprovalRepository : IGenericRepository<ApprovalTemplate> { }
}
