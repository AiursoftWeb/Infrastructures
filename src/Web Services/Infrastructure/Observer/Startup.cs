using Aiursoft.Archon.SDK.Services;
using Aiursoft.Pylon;
using Aiursoft.SDK;
using Aiursoft.Observer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Observer
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
            services.AddAiurDependencies();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurAPIHandler(env.IsDevelopment());
            app.UseStaticFiles();
            app.UseAiursoftDefault();
        }
    }
}
