using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.SDK;

public static class ProgramExtends
{
    public static async Task<IHost> UpdateDbAsync<TContext>(this IHost host) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();
        logger.LogInformation("Migrating database associated with context {ContextName}", typeof(TContext).Name);

        try
        {
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("The database with context {ContextName} was ensured to be created",
                typeof(TContext).Name);

            await context.Database.MigrateAsync();
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