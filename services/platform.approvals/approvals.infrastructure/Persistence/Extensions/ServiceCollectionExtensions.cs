using approvals.application.Interfaces;
using approvals.infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YourProject.Infrastructure.Extensions
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
                        options.UseSqlServer(connectionStrings["SqlServer"]);
                        break;
                    case "SqlLocalDb":
                        options.UseSqlServer(connectionStrings["SqlLocalDb"]);
                        break;
                    case "Postgres":
                        options.UseNpgsql(connectionStrings["Postgres"]);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported provider: {provider}");
                }
            });

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            return services;
        }
    }
}
