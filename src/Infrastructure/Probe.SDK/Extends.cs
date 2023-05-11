using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Aiursoft.Probe.SDK.Services;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.Scanner;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK;

public static class Extends
{
    public static IServiceCollection AddProbeServer(
        this IServiceCollection services,
        string serverEndpoint)
    {
        AsyncHelper.TryAsync(async () =>
        {
            var serverConfigString = await SimpleHttp.DownloadAsString(serverEndpoint);
            var serverConfig = JsonConvert.DeserializeObject<IndexViewModel>(serverConfigString);
            var openFormat = serverConfig.OpenPattern;
            var downloadFormat = serverConfig.DownloadPattern;
            var playerFormat = serverConfig.PlayerPattern;
            services.AddSingleton(new ProbeLocator(serverEndpoint, openFormat, downloadFormat, playerFormat));
        }, 5);

        services.AddLibraryDependencies();
        return services;
    }

    public static IHost InitSite<TProvider>(this IHost host,
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
        var cannon = services.GetRequiredService<CannonService>();
        cannon.Fire<SitesService>(sitesService =>
        {
            cannon.FireAsync<TProvider>(async tokenProvider =>
            {
                // Wait 20 seconds. Dependencies might not be started yet.
                await Task.Delay(20 * 1000);
                var token = await getToken(tokenProvider);
                logger.LogInformation("Starting create Probe resources...");

                var sites = await sitesService.ViewMySitesAsync(token);
                if (!sites.Sites.Any(s => s.SiteName == siteName))
                {
                    await sitesService.CreateNewSiteAsync(token, siteName, openToUpload, openToDownload);
                }
            });
        });

        return host;
    }
}