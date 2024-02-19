using Aiursoft.Directory.SDK;
using Aiursoft.Observer.SDK;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.Models.Configuration;
using Aiursoft.Probe.SDK;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Aiursoft.Probe.Services;
using Aiursoft.SDK;
using Aiursoft.WebTools.Abstractions.Models;
using Microsoft.AspNetCore.Http.Features;

namespace Aiursoft.Probe;

public class Startup : IWebStartup
{
    public virtual void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.Configure<FormOptions>(x => x.MultipartBodyLengthLimit = long.MaxValue);
        services.Configure<ProbeDownloadPatternConfig>(configuration.GetSection("DownloadPatternConfig"));
        services.Configure<DiskAccessConfig>(configuration.GetSection("DiskAccessConfig"));

        services.AddDbContextForInfraApps<ProbeDbContext>(configuration.GetConnectionString("DatabaseConnection"));

        services.AddAiursoftWebFeatures();
        services.AddAiursoftAppAuthentication(configuration.GetSection("AiursoftAuthentication"));
        services.AddAiursoftObserver(configuration.GetSection("AiursoftObserver"));
        services.AddAiursoftProbe(configuration.GetSection("AiursoftProbe"));

        services.AddScannedServices();
        services.AddScoped<IStorageProvider, DiskAccess>();
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler(addUserFriendlyPages: true);
        app.UseCors(builder => builder.AllowAnyOrigin());
        app.UseAiursoftAPIAppRouters();
    }
}