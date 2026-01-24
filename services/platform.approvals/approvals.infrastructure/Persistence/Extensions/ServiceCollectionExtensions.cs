using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.application.Interfaces.Services;
using approvals.application.Services;
using approvals.infrastructure.Persistence;
using approvals.infrastructure.Persistence.Repositories;
using approvals.infrastructure.Persistence.Repositories.Base;
using approvals.infrastructure.Persistence.UnitOfWork;
using approvals.shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace platform.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            var provider = config["DatabaseProvider"];
            var connectionStrings = config.GetSection("ConnectionStrings");

            services.AddDbContext<AppDbContext>(options =>
            {
                switch (provider)
                {
                    case "SqlServer":
                        options.UseSqlServer(connectionStrings["SqlServer"],
                            sql => sql.EnableRetryOnFailure()); 
                        break;

                    case "SqlLocalDb":
                        options.UseSqlServer(connectionStrings["SqlLocalDb"],
                            sql => sql.EnableRetryOnFailure());
                        break;

                    case "Postgres":
                        options.UseNpgsql(connectionStrings["Postgres"]);
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported database provider: {provider}");
                }
            });

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IApprovalRepository, ApprovalRepository>();
            services.AddScoped<IStageDefinitionRepository, StageDefinitionRepository>();
            services.AddScoped<IApprovalInstanceRepository, ApprovalInstanceRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITemplateHubService, TemplateHubService>();
            services.AddScoped<IStageInstanceRepository, StageInstanceRepository>();

            return services;
        }
    }
}
