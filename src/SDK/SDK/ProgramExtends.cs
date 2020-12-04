using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Aiursoft.SDK
{
    public static class ProgramExtends
    {
        public static IHost Update<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder = null) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();
            try
            {
                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
                AsyncHelper.TryAsync(async () =>
                {
                    if (EntryExtends.IsInUT())
                    {
                        await context.Database.EnsureDeletedAsync();
                    }
                    else
                    {
                        await context.Database.MigrateAsync();
                    }
                    seeder?.Invoke(context, services);
                }, 3, (e) =>
                {
                    logger.LogCritical(e, "Update database failed.");
                });
                logger.LogInformation($"Updated database associated with context {typeof(TContext).Name}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
            }

            return host;
        }
    }
}
