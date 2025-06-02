using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.application.Interfaces.Services
{
    public interface IAppDbContext
    {
        DbSet<ApplicationType> ApplicationTypes { get; set; }
        DbSet<FormDefinition> formDefinitions { get; set; }
        DbSet<ApplicationFlowDefinition> applicationFlowDefinitions { get; set; }
    }
}
