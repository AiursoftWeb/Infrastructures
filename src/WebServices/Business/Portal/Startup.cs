using Aiursoft.Portal.Data;
using Aiursoft.Portal.Models;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Stargate.SDK;
using Aiursoft.Warpgate.SDK;
using Aiursoft.WebTools.Abstractions.Models;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Portal;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<PortalDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<PortalUser, IdentityRole>()
            .AddEntityFrameworkStores<PortalDbContext>()
            .AddDefaultTokenProviders();
        services.AddAiursoftWebFeatures();
        services.AddAiursoftWarpgate(configuration.GetSection("AiursoftWarpgate"));
        services.AddAiursoftStargate(configuration.GetSection("AiursoftStargate"));
        services.AddAiursoftIdentity<PortalUser>(
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