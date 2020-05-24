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
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Aiursoft.Probe.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddProbeServer(
            this IServiceCollection services,
            string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                serverEndpoint = "https://probe.aiursoft.com";
            }

            AsyncHelper.TryAsyncThreeTimes(async () =>
            {
                var serverConfigString = await new WebClient().DownloadStringTaskAsync(serverEndpoint);
                var serverConfig = JsonConvert.DeserializeObject<IndexViewModel>(serverConfigString);
                var openFormat = serverConfig.OpenPattern;
                var downloadFormat = serverConfig.DownloadPattern;
                services.AddSingleton(new ProbeLocator(serverEndpoint, openFormat, downloadFormat));
            });

            services.AddLibraryDependencies();
            return services;
        }

        public static IHost InitSite<TokenProvider>(this IHost host,
            Func<IConfiguration, string> getConfig,
            Func<TokenProvider, Task<string>> getToken,
            bool openToUpload = true,
            bool openToDownload = true) where TokenProvider : class
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TokenProvider>>();
                var siteName = getConfig(configuration);
                var sitesService = services.GetService<SitesService>();
                var tokenProvider = services.GetService(typeof(TokenProvider)) as TokenProvider;
                Task.Factory.StartNew(async () =>
                {
                    // Wait 20 seconds. Dependencies might not be started yet.
                    await Task.Delay(20000);
                    logger.LogInformation("Starting create Probe resources...");
                    var token = await getToken(tokenProvider);
                    var sites = await sitesService.ViewMySitesAsync(token);
                    if (!sites.Sites.Any(s => s.SiteName == siteName))
                    {
                        await sitesService.CreateNewSiteAsync(token, siteName, openToUpload, openToDownload);
                    }
                });
            }

            return host;
        }
    }
}
