using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<ApplicationType> applicationType { get; set; }
        DbSet<FormDefinition> formDefinitions { get; set; }
        DbSet<ApplicationFlowDefinition> applicationFlowDefinitions { get; set; }
    }
}
