using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.Models.Configuration;
using Aiursoft.Warpgate.SDK;
using Aiursoft.WebTools.Models;

namespace Aiursoft.Warpgate;

public class Startup : IWebStartup
{
    public virtual void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.Configure<RedirectConfiguration>(configuration.GetSection("RedirectConfig"));
        services.AddDbContextForInfraApps<WarpgateDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddAiurMvc();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftWarpgate(configuration.GetSection("AiursoftWarpgate"));
        services.AddAiursoftSdk();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment(), addUserFriendlyPages: true);
        app.UseAiursoftAPIAppRouters();
    }
}