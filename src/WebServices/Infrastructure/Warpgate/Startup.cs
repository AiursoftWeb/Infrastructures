using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.Models.Configuration;
using Aiursoft.Warpgate.SDK;

namespace Aiursoft.Warpgate;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.Configure<RedirectConfiguration>(Configuration.GetSection("RedirectConfig"));
        services.AddDbContextForInfraApps<WarpgateDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddAiurMvc();
        services.AddAiursoftAppAuthentication(Configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftWarpgate(Configuration.GetSection("AiursoftWarpgate"));

        services.AddAiursoftSdk();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment(), addUserFriendlyPages: true);
        app.UseAiursoftAPIAppRouters();
    }
}