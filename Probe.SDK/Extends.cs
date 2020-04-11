using Aiursoft.Probe.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Probe.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddProbeServer(
            this IServiceCollection services,
            string serverEndpoint = null,
            string openCDN = null,
            string downloadCDN = null,
            string probeIO = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                serverEndpoint = "https://probe.aiursoft.com";
            }
            if (string.IsNullOrWhiteSpace(openCDN))
            {
                openCDN = "https://probe.cdn.aiursoft.com/{0}";
            }
            if (string.IsNullOrWhiteSpace(downloadCDN))
            {
                downloadCDN = "https://probe.download.cdn.aiursoft.com/{0}";
            }
            if (string.IsNullOrWhiteSpace(probeIO))
            {
                probeIO = "https://{0}.aiur.site";
            }
            services.AddSingleton(new ProbeLocator(serverEndpoint, openCDN, downloadCDN, probeIO));
            return services;
        }
    }
}
