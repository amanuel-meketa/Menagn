using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.application.Interfaces.Services
{
    public interface IAppDbContext
    {
        public DbSet<ApprovalTemplate> ApprovalTemplates { get; set; } 
        public DbSet<StageDefinition> StageDefinitions { get; set; } 
        public DbSet<ApprovalInstance> ApprovalInstances { get; set; } 
        public DbSet<StageInstance> StageInstances { get; set; }
    }
}
