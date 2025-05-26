using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace approvals.infrastructure.Persistence
{
    public static class MigrationExtension
    {
        public static async Task EnsureMigrationAppliedAsync(this IApplicationBuilder app, IHostEnvironment env)
        {
            if (!env.IsDevelopment())
                return;

            using var scope = app.ApplicationServices.CreateScope(); 
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
