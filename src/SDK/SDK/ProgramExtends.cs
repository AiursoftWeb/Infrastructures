using Aiursoft.XelNaga.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Aiursoft.SDK
{
    public static class ProgramExtends
    {
        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder = null) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();
            var configuration = services.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DatabaseConnection");
            try
            {
                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
                logger.LogInformation($"Connection string is {connectionString}");
                AsyncHelper.TryAsync(async () =>
                {
                    await context.Database.MigrateAsync();
                    seeder?.Invoke(context, services);
                }, 3, (e) =>
                {
                    logger.LogCritical(e, "Seed database failed.");
                });
                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
            }

            return host;
        }
    }
}
