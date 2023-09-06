using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.WebTools.Models;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Aiursoft.Wiki.Services;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Wiki;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<WikiDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<WikiUser, IdentityRole>()
            .AddEntityFrameworkStores<WikiDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiursoftWebFeatures();

        services.AddAiursoftIdentity<WikiUser>(
            probeConfig: configuration.GetSection("AiursoftProbe"),
            authenticationConfig: configuration.GetSection("AiursoftAuthentication"),
            observerConfig: configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseAiursoftAppRouters();
    }
}

public static class PostStartUp
{
    public static async Task<IHost> SeedAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Seeder>>();
        var seeder = services.GetRequiredService<Seeder>();
        logger.LogInformation("Seeding...");
        await seeder.SeedWithRetry();
        logger.LogInformation("Seeded");
        return host;
    }
}