using Aiursoft.Scanner;
using Aiursoft.Stargate.SDK.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Stargate.SDK;

public static class Extends
{
    public static IServiceCollection AddAiursoftStargate(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<StargateConfiguration>(configurationSection);
        services.AddLibraryDependencies();
        return services;
    }

    public static IServiceCollection AddAiursoftStargate(
        this IServiceCollection services,
        string endPointUrl)
    {
        services.Configure<StargateConfiguration>(options => options.Instance = endPointUrl);
        services.AddLibraryDependencies();
        return services;
    }
}