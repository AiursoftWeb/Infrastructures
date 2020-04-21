using Aiursoft.Archon.SDK.Services;
using Aiursoft.Scanner;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            services.AddLibraryDependencies();
            return services;
        }
    }
}
