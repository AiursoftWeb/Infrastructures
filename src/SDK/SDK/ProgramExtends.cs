using System;
using System.Threading.Tasks;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
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
        try
        {
            logger.LogInformation("Migrating database associated with context {ContextName}", typeof(TContext).Name);

            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Update database with context {ContextName} failed", typeof(TContext).Name);
                throw;
            }

            logger.LogInformation("Updated database associated with context {ContextName}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An error occurred while migrating the database used on context {ContextName}", typeof(TContext).Name);
        }

        return host;
    }
}