using Aiursoft.Probe.SDK.Services;
using Aiursoft.Scanner;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Probe.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddProbeServer(
            this IServiceCollection services,
            string serverEndpoint = null,
            string openFormat = null,
            string downloadFormat = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                serverEndpoint = "https://probe.aiursoft.com";
            }
            if (string.IsNullOrWhiteSpace(openFormat))
            {
                openFormat = "https://{0}.aiur.site";
            }
            if (string.IsNullOrWhiteSpace(downloadFormat))
            {
                downloadFormat = "https://{0}.download.aiur.site";
            }
            services.AddSingleton(new ProbeLocator(serverEndpoint, openFormat, downloadFormat));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
