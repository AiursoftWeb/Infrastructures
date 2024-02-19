using Aiursoft.Account.Data;
using Aiursoft.Account.Models;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.WebTools.Abstractions.Models;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Account;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<AccountDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<AccountUser, IdentityRole>()
            .AddEntityFrameworkStores<AccountDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiursoftWebFeatures();
        services.AddAiursoftIdentity<AccountUser>(
            probeConfig: configuration.GetSection("AiursoftProbe"),
            authenticationConfig: configuration.GetSection("AiursoftAuthentication"),
            observerConfig: configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler();
        app.UseAiursoftAppRouters();
    }
}