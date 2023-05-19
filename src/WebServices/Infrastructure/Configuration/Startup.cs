using Aiursoft.Configuration.Data;
using Aiursoft.Gateway.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Configuration;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextWithCache<ConfigurationDbContext>(Configuration.GetConnectionString("DatabaseConnection"));
        services.AddAiurAPIMvc();
        services.AddGatewayServer(Configuration.GetConnectionString("GatewayConnection"));
        services.AddObserverServer(Configuration.GetConnectionString("ObserverConnection"));
        services.AddAiursoftSDK();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiurAPIHandler(env.IsDevelopment());
        app.UseCors(builder => builder.AllowAnyOrigin());
        app.UseAiursoftAPIDefault();
    }
}