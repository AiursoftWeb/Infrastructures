using Aiursoft.Configuration.Data;
using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.WebTools.Abstractions.Models;

namespace Aiursoft.Configuration;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<ConfigurationDbContext>(configuration.GetConnectionString("DatabaseConnection"));
        services.AddAiursoftWebFeatures();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddScannedServices();
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler(addUserFriendlyPages: false);
        app.UseAiursoftAPIAppRouters();
    }
}