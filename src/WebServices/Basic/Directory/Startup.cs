using System;
using Aiursoft.Directory.Models;
using Aiursoft.Identity;
using Aiursoft.Identity.Services;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.SDK;
using Edi.Captcha;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Aiursoft.Probe.SDK;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;

namespace Aiursoft.Directory;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextWithCache<DirectoryDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
        });

        services.AddIdentity<DirectoryUser, IdentityRole>(options => options.Password = AuthValues.PasswordOptions)
            .AddEntityFrameworkStores<DirectoryDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiurMvc();
    
        // After having gateway, these should be migrated.
        services.AddAiursoftAppAuthentication(Configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftProbe(Configuration.GetSection("AiursoftProbe"));

        // After having gateway, these should be migrated.
        services.AddAiursoftSdk(abstracts: typeof(IAuthProvider));
        services.AddScoped<UserImageGenerator<DirectoryUser>>();
        services.AddSessionBasedCaptcha();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        
        // TODO: After having gateway, this should be migrated.
        app.UseSession();
        
        // TODO: This should be an API project! Migrate the user logic to gateway soon!
        app.UseAiursoftAppRouters();
    }
}