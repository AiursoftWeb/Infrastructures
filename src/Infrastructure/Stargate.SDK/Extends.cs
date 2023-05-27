using Aiursoft.Scanner;
using Aiursoft.Stargate.SDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Stargate.SDK;

public static class Extends
{
    public static IServiceCollection AddStargateServer(this IServiceCollection services, string serverEndpoint)
    {
        // TODO: Use configuration!
        services.AddSingleton(new StargateLocator(serverEndpoint));
        services.AddLibraryDependencies();
        return services;
    }
}