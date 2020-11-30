using Aiursoft.Archon.SDK.Services;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Wrap.Data;
using Aiursoft.Wrap.Models;
using Aiursoft.Wrapgate.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Wrap
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["WrapAppId"];
            AppsContainer.CurrentAppSecret = configuration["WrapAppSecret"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<WrapDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

            services.AddIdentity<WrapUser, IdentityRole>()
                .AddEntityFrameworkStores<WrapDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiurMvc();
            services.AddWrapgateServer(Configuration.GetConnectionString("WrapgateConnection"));
            services.AddAiursoftIdentity<WrapUser>(
                archonEndpoint: Configuration.GetConnectionString("ArchonConnection"),
                observerEndpoint: Configuration.GetConnectionString("ObserverConnection"),
                probeEndpoint: Configuration.GetConnectionString("ProbeConnection"),
                gateEndpoint: Configuration.GetConnectionString("GatewayConnection"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(env.IsDevelopment());
            app.UseAiursoftDefault();
        }
    }
}
