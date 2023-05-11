using System;
using System.Threading.Tasks;
using Aiursoft.DBTools.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.SDK;

public static class ProgramExtends
{
    public static IHost Update<TContext>(this IHost host) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();
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

                var seeder = services.GetService<ISeeder>();
                seeder?.Seed();
            }, 3, e =>
            {
                logger.LogCritical(e, "Update database failed.");
                return Task.CompletedTask;
            });
            logger.LogInformation($"Updated database associated with context {typeof(TContext).Name}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
        }

        return host;
    }
}