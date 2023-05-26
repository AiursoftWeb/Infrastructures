using Aiursoft.Developer.SDK.Services;
using Aiursoft.Scanner;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aiursoft.Developer.SDK;

public static class Extends
{
    [Obsolete(error: false, message: "Aiursoft Developer Server will be merged to Directory!")]
    public static IServiceCollection AddDeveloperServer(this IServiceCollection services, string serverEndpoint)
    {
        services.AddSingleton(new DeveloperLocator(serverEndpoint));
        services.AddLibraryDependencies();
        return services;
    }
}