using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK.Services;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Observer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        AppsContainer.CurrentAppId = configuration["ObserverAppId"];
        AppsContainer.CurrentAppSecret = configuration["ObserverAppSecret"];
    }

    public IConfiguration Configuration { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextWithCache<ObserverDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddAiurAPIMvc();
        services.AddAiursoftSDK();
        services.AddArchonServer(Configuration.GetConnectionString("ArchonConnection"));
        services.AddSingleton(new ObserverLocator(Configuration["ObserverEndpoint"]));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiurAPIHandler(env.IsDevelopment());
        app.UseAiursoftAPIDefault();
    }
}