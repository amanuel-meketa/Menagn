using approvals.application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
