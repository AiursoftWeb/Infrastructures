using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.SDK;
using Aiursoft.Developer.SDK;
using Aiursoft.SDK;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace Aiursoft.Archon
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            AppsContainer.CurrentAppId = configuration["ArchonAppId"];
            AppsContainer.CurrentAppSecret = configuration["ArchonAppSecret"];
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAiurAPIMvc();
            services.AddDeveloperServer();
            services.AddSingleton(new ArchonLocator(Configuration["ArchonEndpoint"], new RSAParameters
            {
                Modulus = Configuration["Key:Modulus"].Base64ToBytes(),
                Exponent = Configuration["Key:Exponent"].Base64ToBytes()
            }));
            services.AddObserverServer();
            services.AddBasic();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurAPIHandler(env.IsDevelopment());
            app.UseAiursoftAPIDefault();
        }
    }
}
