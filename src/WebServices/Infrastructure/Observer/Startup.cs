using Aiursoft.Directory.SDK;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK;
using Aiursoft.SDK;

namespace Aiursoft.Observer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextForInfraApps<ObserverDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

        services.AddAiurMvc();
        services.AddAiursoftSdk();
        services.AddAiursoftAppAuthentication(Configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment(), addUserFriendlyPages: false);
        app.UseAiursoftAPIAppRouters();
    }
}