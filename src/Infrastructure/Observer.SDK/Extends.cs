using Aiursoft.Observer.SDK.Services;
using Aiursoft.Scanner;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Observer.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddObserverServer(this IServiceCollection services, string serverEndpoint)
        {
            services.AddSingleton(new ObserverLocator(serverEndpoint));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
