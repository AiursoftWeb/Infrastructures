using Aiursoft.Observer.SDK.Configuration;
using Aiursoft.Observer.SDK.Services;
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
}