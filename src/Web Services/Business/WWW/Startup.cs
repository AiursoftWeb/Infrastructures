using Aiursoft.Archon.SDK.Services;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.WWW.Data;
using Aiursoft.WWW.Models;
using Aiursoft.WWW.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.WWW
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["WWWAppId"];
            AppsContainer.CurrentAppSecret = configuration["WWWAppSecret"];
            BingTranslator.APIKey = configuration["TranslateAPIKey"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<WWWDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

            services.AddIdentity<WWWUser, IdentityRole>()
                .AddEntityFrameworkStores<WWWDbContext>()
                .AddDefaultTokenProviders();
            services.AddCors();
            services.AddAiurMvc();
            services.AddAiurDependenciesWithIdentity<WWWUser>(
                archonEndpoint: Configuration.GetConnectionString("ArchonConnection"),
                observerEndpoint: Configuration.GetConnectionString("ObserverConnection"),
                probeEndpoint: Configuration.GetConnectionString("ProbeConnection"),
                gateEndpoint: Configuration.GetConnectionString("GatewayConnection"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(env.IsDevelopment());
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseAiursoftDefault();
        }
    }
}
