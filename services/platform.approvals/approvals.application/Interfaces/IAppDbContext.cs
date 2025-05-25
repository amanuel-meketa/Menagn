using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Approval> Approvals { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
