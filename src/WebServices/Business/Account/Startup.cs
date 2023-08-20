using Aiursoft.Account.Data;
using Aiursoft.Account.Models;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Account;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextForInfraApps<AccountDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<AccountUser, IdentityRole>()
            .AddEntityFrameworkStores<AccountDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiurMvc();
        services.AddAiursoftIdentity<AccountUser>(
            probeConfig: Configuration.GetSection("AiursoftProbe"),
            authenticationConfig: Configuration.GetSection("AiursoftAuthentication"),
            observerConfig: Configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseAiursoftAppRouters();
    }
}