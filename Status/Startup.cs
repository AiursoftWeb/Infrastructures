using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToArchonServer;
using Aiursoft.Status.Data;
using Aiursoft.Status.Services.Aiursoft.Probe.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Status
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
            services.AddDbContext<StatusDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services
                .AddControllersWithViews()
                .AddNewtonsoftJson()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.AddTokenManager();
            services.AddSingleton<IHostedService, TimedChecker>();
            services.AddSingleton<ServiceLocation>();
            services.AddSingleton<Counter>();
            services.AddSingleton<AppsContainer>();
            services.AddHttpClient();
            services.AddScoped<ArchonApiService>();
            services.AddScoped<HTTPService>();
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
                app.UseAPIFriendlyErrorPage();
            }
            app.UseAiursoftSupportedCultures();
            app.UseStaticFiles();
            app.UseLanguageSwitcher();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
            app.UseDocGenerator();
        }
    }
}
