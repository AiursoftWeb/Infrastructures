using System.Collections.Generic;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Developer.SDK;
using Aiursoft.Gateway.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Aiursoft.Stargate.SDK;
using Aiursoft.Status.Models;
using Aiursoft.Warpgate.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Status;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        AppsContainer.CurrentAppId = configuration["StatusAppId"];
        AppsContainer.CurrentAppSecret = configuration["StatusAppSecret"];
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<List<MonitorRule>>(Configuration.GetSection("CustomRules"));
        services.AddAiurMvc();
        services.AddGatewayServer(Configuration.GetConnectionString("GatewayConnection"));
        services.AddObserverServer(Configuration.GetConnectionString("ObserverConnection"));
        services.AddStargateServer(Configuration.GetConnectionString("StargateConnection"));
        services.AddDeveloperServer(Configuration.GetConnectionString("DeveloperConnection"));
        services.AddWarpgateServer(Configuration.GetConnectionString("WarpgateConnection"));
        services.AddProbeServer(Configuration.GetConnectionString("ProbeConnection"));
        services.AddAiursoftSDK();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiurUserHandler(env.IsDevelopment());
        app.UseAiursoftDefault();
    }
}