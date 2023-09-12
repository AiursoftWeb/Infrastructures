using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK;
using Aiursoft.WebTools.Models;

namespace Aiursoft.Stargate;

public class Startup : IWebStartup
{
    public virtual void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<StargateDbContext>(configuration.GetConnectionString("DatabaseConnection"));
        services.AddAiursoftWebFeatures();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftStargate(configuration.GetSection("AiursoftStargate"));
        services.AddScannedServices();
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler(addUserFriendlyPages: false);
        app.UseWebSockets();
        app.UseAiursoftAPIAppRouters();
    }
}