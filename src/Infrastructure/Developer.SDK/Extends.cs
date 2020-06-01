using Aiursoft.Developer.SDK.Services;
using Aiursoft.Scanner;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Developer.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddDeveloperServer(this IServiceCollection services, string serverEndpoint)
        {
            services.AddSingleton(new DeveloperLocator(serverEndpoint));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
