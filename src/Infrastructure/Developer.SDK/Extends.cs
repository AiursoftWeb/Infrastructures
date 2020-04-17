using Aiursoft.Developer.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Developer.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddDeveloperServer(this IServiceCollection services, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft developer server.
                serverEndpoint = "https://developer.aiursoft.com";
            }
            services.AddSingleton(new DeveloperLocator(serverEndpoint));
            return services;
        }
    }
}
