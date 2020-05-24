using Aiursoft.Archon.SDK.Services;
using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Pylon;
using Aiursoft.Developer.SDK;
using Aiursoft.SDK;
using Edi.Captcha;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Aiursoft.Gateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppsContainer.CurrentAppId = configuration["GatewayAppId"];
            AppsContainer.CurrentAppSecret = configuration["GatewayAppSecret"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextWithCache<GatewayDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            services.AddIdentity<GatewayUser, IdentityRole>(options => options.Password = Values.PasswordOptions)
                .AddEntityFrameworkStores<GatewayDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiurMvc();
            services.AddDeveloperServer();
            services.AddAiurDependenciesWithIdentity<GatewayUser>();
            services.AddSessionBasedCaptcha();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(env.IsDevelopment());
            app.UseSession();
            app.UseAiursoftDefault();
        }
    }
}
