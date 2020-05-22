using Aiursoft.Scanner;
using Aiursoft.Observer.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Observer.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddObserverServer(this IServiceCollection services, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft obverver server.
                serverEndpoint = "https://status.aiursoft.com";
            }
            services.AddSingleton(new StatusLocator(serverEndpoint));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
