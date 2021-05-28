using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Developer.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Aiursoft.Stargate.SDK;
using Aiursoft.Status.Data;
using Aiursoft.Warpgate.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Status
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["StatusAppId"];
            AppsContainer.CurrentAppSecret = configuration["StatusAppSecret"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<StatusDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

            services.AddAiurMvc();
            services.AddArchonServer(Configuration.GetConnectionString("ArchonConnection"));
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
}
