using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Scanner;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Gateway.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddGatewayServer(this IServiceCollection services, string serverEndpoint)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft gateway server.
                serverEndpoint = "https://gateway.aiursoft.com";
            }
            services.AddSingleton(new GatewayLocator(serverEndpoint));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
