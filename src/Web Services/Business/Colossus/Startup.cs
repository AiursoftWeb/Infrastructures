using Aiursoft.Archon.SDK.Services;
using Aiursoft.Colossus.Data;
using Aiursoft.Colossus.Models;
using Aiursoft.Pylon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Colossus
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["ColossusAppId"];
            AppsContainer.CurrentAppSecret = configuration["ColossusAppSecret"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ColossusDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddIdentity<ColossusUser, IdentityRole>()
                .AddEntityFrameworkStores<ColossusDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiurMvc();

            services.AddAiurDependenciesWithIdentity<ColossusUser>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(env.IsDevelopment());
            app.UseAiursoftDefault();
        }
    }
}
