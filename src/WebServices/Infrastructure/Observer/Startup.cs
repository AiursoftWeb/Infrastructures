using Aiursoft.Directory.SDK;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.WebTools.Abstractions.Models;

namespace Aiursoft.Observer;

public class Startup : IWebStartup
{
    public virtual void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<ObserverDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddAiursoftWebFeatures();
        services.AddScannedServices();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler(addUserFriendlyPages: false);
        app.UseAiursoftAPIAppRouters();
    }
}