using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.Scanner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Probe.SDK;

public static class Extends
{
    public static IServiceCollection AddAiursoftProbe(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<ProbeConfiguration>(configurationSection);
        services.AddLibraryDependencies();
        return services;
    }

    public static IServiceCollection AddAiursoftProbe(
        this IServiceCollection services,
        string endPointUrl)
    {
        services.Configure<ProbeConfiguration>(options => options.Instance = endPointUrl);
        services.AddLibraryDependencies();
        return services;
    }

    public static async Task<IHost> InitSiteAsync<TProvider>(this IHost host,
        Func<IConfiguration, string> getConfig,
        Func<TProvider, Task<string>> getToken,
        bool openToUpload = true,
        bool openToDownload = true) where TProvider : class
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<TProvider>>();
        var siteName = getConfig(configuration);
        var sitesService = services.GetRequiredService<SitesService>();
        var tokenProvider = services.GetRequiredService<TProvider>();
        var token = await getToken(tokenProvider);

        logger.LogInformation("Getting Probe sites...");
        var sites = await sitesService.ViewMySitesAsync(token);

        if (sites.Sites.All(s => s.SiteName != siteName))
        {
            logger.LogInformation("Creating new Probe site...");
            await sitesService.CreateNewSiteAsync(token, siteName, openToUpload, openToDownload);
        }

        return host;
    }
}