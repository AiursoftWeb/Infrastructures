using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.Models.Configuration;
using Aiursoft.Warpgate.SDK;
using Aiursoft.WebTools.Abstractions.Models;

namespace Aiursoft.Warpgate;

public class Startup : IWebStartup
{
    public virtual void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.Configure<RedirectConfiguration>(configuration.GetSection("RedirectConfig"));
        services.AddDbContextForInfraApps<WarpgateDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddAiursoftWebFeatures();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftWarpgate(configuration.GetSection("AiursoftWarpgate"));
        services.AddScannedServices();
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler(addUserFriendlyPages: true);
        app.UseAiursoftAPIAppRouters();
    }
}