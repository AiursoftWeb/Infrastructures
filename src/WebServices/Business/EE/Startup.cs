using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.EE;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextForInfraApps<EEDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddIdentity<EEUser, IdentityRole>()
            .AddEntityFrameworkStores<EEDbContext>()
            .AddDefaultTokenProviders();

        services.AddAiurMvc();
        services.AddAiursoftIdentity<EEUser>(
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