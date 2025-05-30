using approvals.application.Interfaces;
using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<FormDefinition> formDefinitions { get; set; }
        public DbSet<ApplicationFlowDefinition> applicationFlowDefinitions { get; set; }

    }
}
