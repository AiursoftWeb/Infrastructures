using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Stargate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["TestAppId"];
            AppsContainer.CurrentAppSecret = configuration["TestAppSecret"];
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<StargateDbContext>(Configuration.GetConnectionString("DatabaseConnection"));
            services.AddAiurAPIMvc();
            services.AddArchonServer(Configuration.GetConnectionString("ArchonConnection"));
            services.AddObserverServer(Configuration.GetConnectionString("ObserverConnection"));
            services.AddAiursoftSDK();
            services.AddSingleton(new StargateLocator(endpoint: Configuration["StargateEndpoint"]));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurAPIHandler(env.IsDevelopment());
            app.UseWebSockets();
            app.UseAiursoftAPIDefault();
        }
    }
}
