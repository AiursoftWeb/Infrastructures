using Aiursoft.Gateway.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Gateway.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddGatewayServer(this IServiceCollection services, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft gateway server.
                serverEndpoint = "https://gateway.aiursoft.com";
            }
            services.AddSingleton(new GatewayLocator(serverEndpoint));
            return services;
        }
    }
}
