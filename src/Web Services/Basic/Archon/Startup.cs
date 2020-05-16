using Aiursoft.Archon.SDK.Services;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Archon
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AppsContainer.CurrentAppId = configuration["ArchonAppId"];
            AppsContainer.CurrentAppSecret = configuration["ArchonAppSecret"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAiurAPIMvc();
            services.AddAiurDependencies(addProbe: false, addArchon: false);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurAPIHandler(env.IsDevelopment());
            app.UseAiursoftAPIDefault();
        }
    }
}
