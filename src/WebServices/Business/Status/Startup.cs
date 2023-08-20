using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Aiursoft.Stargate.SDK;
using Aiursoft.Status.Models;
using Aiursoft.Warpgate.SDK;

namespace Aiursoft.Status;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<List<MonitorRule>>(Configuration.GetSection("CustomRules"));
        services.AddAiurMvc();
        services.AddAiursoftAppAuthentication(Configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftProbe(Configuration.GetSection("AiursoftProbe"));
        services.AddAiursoftStargate(Configuration.GetSection("AiursoftStargate"));
        services.AddAiursoftWarpgate(Configuration.GetSection("AiursoftWarpgate"));
        services.AddAiursoftSdk();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseAiursoftAppRouters();
    }
}