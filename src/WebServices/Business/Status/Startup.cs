using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Aiursoft.Stargate.SDK;
using Aiursoft.Status.Models;
using Aiursoft.Warpgate.SDK;
using Aiursoft.WebTools.Models;

namespace Aiursoft.Status;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.Configure<List<MonitorRule>>(configuration.GetSection("CustomRules"));
        services.AddAiurosftWebFeatures();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftProbe(configuration.GetSection("AiursoftProbe"));
        services.AddAiursoftStargate(configuration.GetSection("AiursoftStargate"));
        services.AddAiursoftWarpgate(configuration.GetSection("AiursoftWarpgate"));
        services.AddScannedServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseAiursoftAppRouters();
    }
}