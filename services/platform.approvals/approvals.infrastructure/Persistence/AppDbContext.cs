using approvals.application.Interfaces.Services;
using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ApprovalTemplate> ApprovalTemplates { get; set; } = null!;
        public DbSet<StageDefinition> StageDefinitions { get; set; } = null!;
        public DbSet<ApprovalInstance> ApprovalInstances { get; set; } = null!;
        public DbSet<StageInstance> StageInstances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ApprovalTemplate -> StageDefinition
            modelBuilder.Entity<ApprovalTemplate>()
                .HasMany(t => t.StageDefinitions)
                .WithOne(sd => sd.Template)
                .HasForeignKey(sd => sd.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApprovalInstance -> StageInstance
            modelBuilder.Entity<ApprovalInstance>(b =>
            {
                b.OwnsOne(ai => ai.CreatedBy);
                b.HasMany(ai => ai.StageInstances)
                 .WithOne(si => si.ApprovalInstance)
                 .HasForeignKey(si => si.ApprovalInstanceId)
                 .OnDelete(DeleteBehavior.Cascade);
            });


            // StageInstance -> StageDefinition
            modelBuilder.Entity<StageInstance>()
                .HasOne(si => si.StageDefinition)
                .WithMany()
                .HasForeignKey(si => si.StageDefId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}