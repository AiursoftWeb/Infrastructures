using Aiursoft.Archon.SDK.Services;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Aiursoft.Wiki.Services;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            services.AddAiursoftIdentity<WikiUser>(
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

    public static class PostStartUp
    {
        public static IHost Seed(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Seeder>>();
            var seeder = services.GetService<Seeder>();
            logger.LogInformation($"Seeding...");
            AsyncHelper.TryAsync(
                times: 3,
                steps:
                    async () =>
                    {
                        await seeder.Seed();
                    },
                onError: 
                    async (e) => 
                    {
                        await seeder.HandleException(e);
                    }
            );
            logger.LogInformation($"Seeded");
            return host;
        }
    }
}
