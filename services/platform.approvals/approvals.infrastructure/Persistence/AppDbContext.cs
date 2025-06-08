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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // ApprovalTemplate
            builder.Entity<ApprovalTemplate>(e =>
            {
                e.ToTable("ApprovalTemplate");
                e.HasKey(x => x.Id);
            });

            // StageDefinition
            builder.Entity<StageDefinition>(e =>
            {
                e.ToTable("StageDefinition");
                e.HasKey(x => x.Id);

                e.HasIndex(x => new { x.TemplateId, x.SequenceOrder })
                 .IsUnique();

                e.HasOne(x => x.Template)
                 .WithMany(t => t.StageDefinitions)
                 .HasForeignKey(x => x.TemplateId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ApprovalInstance
            builder.Entity<ApprovalInstance>(e =>
            {
                e.ToTable("ApprovalInstance");
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Template)
                 .WithMany(t => t.ApprovalInstances)
                 .HasForeignKey(x => x.TemplateId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // StageInstance
            builder.Entity<StageInstance>(e =>
            {
                e.ToTable("StageInstance");
                e.HasKey(x => x.Id);

                e.HasIndex(x => new { x.InstanceId, x.StageDefId })
                 .IsUnique();

                e.HasOne(x => x.Instance)
                 .WithMany(i => i.StageInstances)
                 .HasForeignKey(x => x.InstanceId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.StageDefinition)
                 .WithMany(d => d.StageInstances)
                 .HasForeignKey(x => x.StageDefId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}
