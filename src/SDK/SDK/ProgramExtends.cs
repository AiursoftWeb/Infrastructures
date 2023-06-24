using System;
using System.Threading.Tasks;
using Aiursoft.CSTools.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.SDK;

public static class ProgramExtends
{
    public static async Task<IHost> UpdateDbAsync<TContext>(this IHost host) where TContext : DbContext
    {
        if (EntryExtends.IsInEntityFramework())
        {
            return host;
        }
        
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();
        logger.LogInformation("Migrating database associated with context {ContextName}", typeof(TContext).Name);

        try
        {
            // Some projects may not be able to migrated. So create first.
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("The database with context {ContextName} was ensured to be created",
                typeof(TContext).Name);

            if (EntryExtends.IsInUnitTests())
            {
                // Reset database for unit tests.
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                await context.Database.MigrateAsync();
            }
            logger.LogInformation("Migrated database associated with context {ContextName}", typeof(TContext).Name);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Update database with context {ContextName} failed", typeof(TContext).Name);
            throw;
        }

        return host;
    }
}