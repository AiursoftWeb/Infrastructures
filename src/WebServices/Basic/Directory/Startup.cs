using Aiursoft.Directory.Models;
using Aiursoft.Identity;
using Aiursoft.Identity.Services;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.SDK;
using Edi.Captcha;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Probe.SDK;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.WebTools.Models;

namespace Aiursoft.Directory;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<DirectoryDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
        });

        services.AddIdentity<DirectoryUser, IdentityRole>(options => options.Password = AuthValues.PasswordOptions)
            .AddEntityFrameworkStores<DirectoryDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiurMvc();
    
        // TODO: After having gateway, these should be migrated.
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftProbe(configuration.GetSection("AiursoftProbe"));

        // TODO: After having gateway, these should be migrated.
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