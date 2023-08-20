using Aiursoft.Portal.Data;
using Aiursoft.Portal.Models;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Stargate.SDK;
using Aiursoft.Warpgate.SDK;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Portal;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextForInfraApps<PortalDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<PortalUser, IdentityRole>()
            .AddEntityFrameworkStores<PortalDbContext>()
            .AddDefaultTokenProviders();
        services.AddAiurMvc();
        services.AddAiursoftWarpgate(Configuration.GetSection("AiursoftWarpgate"));
        services.AddAiursoftStargate(Configuration.GetSection("AiursoftStargate"));
        services.AddAiursoftIdentity<PortalUser>(
            probeConfig: Configuration.GetSection("AiursoftProbe"),
            authenticationConfig: Configuration.GetSection("AiursoftAuthentication"),
            observerConfig: Configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseAiursoftAppRouters();
    }
}