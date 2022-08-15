using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Configuration.Data;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Configuration
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["ConfigurationAppId"];
            AppsContainer.CurrentAppSecret = configuration["ConfigurationAppSecret"];
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<ConfigurationDbContext>(Configuration.GetConnectionString("DatabaseConnection"));
            services.AddAiurAPIMvc();
            services.AddArchonServer(Configuration.GetConnectionString("ArchonConnection"));
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
}
