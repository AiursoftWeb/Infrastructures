using Aiursoft.Configuration.Data;
using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;

namespace Aiursoft.Configuration;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextForInfraApps<ConfigurationDbContext>(Configuration.GetConnectionString("DatabaseConnection"));
        services.AddAiurMvc();
        services.AddAiursoftAppAuthentication(Configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftSdk();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment(), addUserFriendlyPages: false);
        app.UseAiursoftAPIAppRouters();
    }
}