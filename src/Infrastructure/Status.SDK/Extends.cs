using Aiursoft.Scanner;
using Aiursoft.Status.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Status.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddStatusServer(this IServiceCollection services, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft status server.
                serverEndpoint = "https://status.aiursoft.com";
            }
            services.AddSingleton(new StatusLocator(serverEndpoint));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
