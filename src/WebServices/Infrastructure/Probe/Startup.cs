using Aiursoft.Gateway.SDK;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Services;
using Aiursoft.Probe.Services;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Probe;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        AppsContainer.CurrentAppId = configuration["ProbeAppId"];
        AppsContainer.CurrentAppSecret = configuration["ProbeAppSecret"];
    }

    public IConfiguration Configuration { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.Configure<FormOptions>(x => x.MultipartBodyLengthLimit = long.MaxValue);

        services.AddDbContextWithCache<ProbeDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddCors();
        services.AddAiurAPIMvc();
        services.AddGatewayServer(Configuration.GetConnectionString("GatewayConnection"));
        services.AddObserverServer(Configuration.GetConnectionString("ObserverConnection"));
        services.AddAiursoftSDK();
        services.AddScoped<IStorageProvider, DiskAccess>();
        services.AddSingleton(new ProbeLocator(
            Configuration["ProbeEndpoint"],
            Configuration["OpenPattern"],
            Configuration["DownloadPattern"],
            Configuration["PlayerPattern"]));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiurAPIHandler(env.IsDevelopment());
        app.UseCors(builder => builder.AllowAnyOrigin());
        app.UseAiursoftAPIDefault();
    }
}