using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Aiursoft.Wiki.Services;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Wiki;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextForInfraApps<WikiDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<WikiUser, IdentityRole>()
            .AddEntityFrameworkStores<WikiDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiurMvc();

        services.AddAiursoftIdentity<WikiUser>(
            probeConfig: Configuration.GetSection("AiursoftProbe"),
            authenticationConfig: Configuration.GetSection("AiursoftAuthentication"),
            observerConfig: Configuration.GetSection("AiursoftObserver"));
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