using Aiursoft.Archon.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Archon.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddArchonServer(this IServiceCollection services, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft archon server.
                serverEndpoint = "https://archon.aiursoft.com";
            }
            services.AddSingleton(new ArchonLocator(serverEndpoint));
            return services;
        }
    }
}
