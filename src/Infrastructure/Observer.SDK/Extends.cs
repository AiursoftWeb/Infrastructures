using Aiursoft.Observer.SDK.Configuration;
using Aiursoft.Scanner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Observer.SDK;

public static class Extends
{
    public static IServiceCollection AddAiursoftObserver(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.Configure<ObserverConfiguration>(configurationSection);
        services.AddLibraryDependencies();
        return services;
    }

    public static IServiceCollection AddAiursoftObserver(this IServiceCollection services, string endPointUrl)
    {
        services.Configure<ObserverConfiguration>(options => options.Instance = endPointUrl);
        services.AddLibraryDependencies();
        return services;
    }
}