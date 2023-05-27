using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Scanner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Directory.SDK;

public static class Extends
{
    public static IServiceCollection AddAiursoftAppAuthentication(this IServiceCollection services, 
        IConfigurationSection configurationSection)
    {
        services.Configure<DirectoryConfiguration>(configurationSection);
        services.AddLibraryDependencies();
        return services;
    }
}