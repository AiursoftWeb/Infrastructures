using Aiursoft.Archon.SDK.Services;
using Aiursoft.Pylon;
using Aiursoft.SDK;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Wiki
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["WikiAppId"];
            AppsContainer.CurrentAppSecret = configuration["WikiAppSecret"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<WikiDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

            services.AddIdentity<WikiUser, IdentityRole>()
                .AddEntityFrameworkStores<WikiDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiurMvc();

            services.AddAiurDependenciesWithIdentity<WikiUser>(
                archonEndpoint: Configuration.GetConnectionString("ArchonConnection"),
                observerEndpoint: Configuration.GetConnectionString("ObserverConnection"),
                probeEndpoint: Configuration.GetConnectionString("ProbeConnection"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(env.IsDevelopment());
            app.UseAiursoftDefault();
        }
    }
}
