using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.WebTools.Abstractions.Models;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.EE;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<EEDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<EEUser, IdentityRole>()
            .AddEntityFrameworkStores<EEDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiursoftWebFeatures();
        services.AddAiursoftIdentity<EEUser>(
            probeConfig: configuration.GetSection("AiursoftProbe"),
            authenticationConfig: configuration.GetSection("AiursoftAuthentication"),
            observerConfig: configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler();
        app.UseAiursoftAppRouters();
    }
}