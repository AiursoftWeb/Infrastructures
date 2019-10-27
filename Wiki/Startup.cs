using Aiursoft.Pylon;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WikiDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddIdentity<WikiUser, IdentityRole>()
                .AddEntityFrameworkStores<WikiDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiurMvc();

            services.AddAiurDependencies<WikiUser>("Wiki");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHandleRobots();
                app.UseEnforceHttps();
                app.UseUserFriendlyErrorPage();
            }
            app.UseAiursoftSupportedCultures();
            app.UseSystemDefault();
        }
    }
}
