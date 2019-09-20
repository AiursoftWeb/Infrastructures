using Aiursoft.API.Data;
using Aiursoft.API.Models;
using Aiursoft.API.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Edi.Captcha;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.API
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
            services.AddDbContext<APIDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddIdentity<APIUser, IdentityRole>(options => options.Password = Values.PasswordOptions)
                .AddEntityFrameworkStores<APIDbContext>()
                .AddDefaultTokenProviders();

            services.AddTokenManager();

            services
                .AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddSingleton<IHostedService, TimedCleaner>();
            services.AddSingleton<ServiceLocation>();
            services.AddScoped<HTTPService>();
            services.AddScoped<DeveloperApiService>();
            services.AddScoped<GrantChecker>();
            services.AddTransient<UserImageGenerator<APIUser>>();
            services.AddTransient<AiurEmailSender>();
            services.AddTransient<APISMSSender>();
            services.AddTransient<ConfirmationEmailSender>();
            services.AddTransient<ISessionBasedCaptcha, BasicLetterCaptcha>();
        }

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
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
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseLanguageSwitcher();
            app.UseMvcWithDefaultRoute();
            app.UseDocGenerator();
        }
    }
}
