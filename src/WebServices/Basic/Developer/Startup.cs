using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Aiursoft.Developer.SDK.Services;
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

namespace Aiursoft.Developer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        AppsContainer.CurrentAppId = configuration["DeveloperAppId"];
        AppsContainer.CurrentAppSecret = configuration["DeveloperAppSecret"];
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextWithCache<DeveloperDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<DeveloperUser, IdentityRole>()
            .AddEntityFrameworkStores<DeveloperDbContext>()
            .AddDefaultTokenProviders();
        services.AddSingleton(new DeveloperLocator(Configuration["DeveloperEndpoint"]));
        services.AddAiurMvc();
        services.AddWarpgateServer(Configuration.GetConnectionString("WarpgateConnection"));
        services.AddStargateServer(Configuration.GetConnectionString("StargateConnection"));
        services.AddAiursoftIdentity<DeveloperUser>(
            Configuration.GetConnectionString("ObserverConnection"),
            Configuration.GetConnectionString("ProbeConnection"),
            Configuration.GetConnectionString("GatewayConnection"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiurUserHandler(env.IsDevelopment());
        app.UseAiursoftDefault();
    }
}