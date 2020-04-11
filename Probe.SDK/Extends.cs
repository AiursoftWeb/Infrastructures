using Aiursoft.Probe.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Probe.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddProbeServer(this IServiceCollection services, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft stargate server.
                serverEndpoint = "https://probe.aiursoft.com";
            }
            services.AddSingleton(new ProbeLocator(serverEndpoint));
            return services;
        }
    }
}
