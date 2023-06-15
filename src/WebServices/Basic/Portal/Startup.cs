using Aiursoft.Portal.Data;
using Aiursoft.Portal.Models;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Stargate.SDK;
using Aiursoft.Warpgate.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        services.AddDbContextWithCache<PortalDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

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