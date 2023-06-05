using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Aiursoft.Wiki.Services;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        services.AddDbContextWithCache<WikiDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

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
        app.UseAiuroftHandler(env.IsDevelopment());
        app.UseAiursoftAppRouters();
    }
}

public static class PostStartUp
{
    public static IHost Seed(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Seeder>>();
        var seeder = services.GetRequiredService<Seeder>();
        logger.LogInformation("Seeding...");
        AsyncHelper.TryAsync(
            times: 3,
            taskFactory: () => seeder.Seed(),
            onError: e => seeder.HandleException(e)
        );
        logger.LogInformation("Seeded");
        return host;
    }
}