using Aiursoft.Pylon;
using Aiursoft.Wiki.Data;
using Aiursoft.Wiki.Models;
using Aiursoft.Wiki.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddMvc();

            services.AddAiursoftAuth<WikiUser>();
            services.AddTransient<Seeder>();
            services.AddTransient<MarkDownGenerator>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
                app.UseExceptionHandler("/Error/ServerException");
                app.UseStatusCodePagesWithReExecute("/Error/Code{0}");
            }
            app.UseAiursoftAuthenticationFromConfiguration(Configuration, "Wiki");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseLanguageSwitcher();
            app.UseMvcWithDefaultRoute();
        }
    }
}
