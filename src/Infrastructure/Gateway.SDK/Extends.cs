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
            services.AddSingleton(new GatewayLocator(serverEndpoint));
            services.AddLibraryDependencies();
            return services;
        }
    }
}
