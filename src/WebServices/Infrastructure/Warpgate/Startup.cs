using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.SDK.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Warpgate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["WarpgateAppId"];
            AppsContainer.CurrentAppSecret = configuration["WarpgateAppSecret"];
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<WarpgateDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

            services.AddAiurAPIMvc();
            services.AddArchonServer(Configuration.GetConnectionString("ArchonConnection"));
            services.AddObserverServer(Configuration.GetConnectionString("ObserverConnection"));
            services.AddSingleton(new WarpgateLocator(
                Configuration["WarpgateEndpoint"], 
                Configuration["WarpPattern"]));
            services.AddAiursoftSDK();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurAPIHandler(env.IsDevelopment());
            app.UseAiursoftAPIDefault();
        }
    }
}
