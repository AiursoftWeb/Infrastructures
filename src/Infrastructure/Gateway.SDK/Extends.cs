using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Scanner;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Aiursoft.Gateway.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddGatewayServer(this IServiceCollection services, string serverEndpoint)
        {
            if (Assembly.GetEntryAssembly().FullName?.StartsWith("ef") ?? false)
            {
                Console.WriteLine("Calling from Entity Framework! Skipped dependencies management!");
                return services;
            }
            services.AddSingleton(new GatewayLocator(serverEndpoint));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
