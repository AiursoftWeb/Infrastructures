using Aiursoft.Scanner;
using Aiursoft.Warpgate.SDK.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Warpgate.SDK;

public static class Extends
{
    public static IServiceCollection AddAiursoftWarpgate(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<WarpgateConfiguration>(configurationSection);
        services.AddLibraryDependencies();
        return services;
    }

    public static IServiceCollection AddAiursoftWarpgate(
        this IServiceCollection services,
        string endPointUrl)
    {
        services.Configure<WarpgateConfiguration>(options => options.Instance = endPointUrl);
        services.AddLibraryDependencies();
        return services;
    }
}