using System.Security.Cryptography;
using Aiursoft.Gateway.SDK.Models.API.HomeViewModels;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Scanner;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.SDK;

public static class Extends
{
    public static IServiceCollection AddGatewayServer(this IServiceCollection services, string serverEndpoint)
    {
        AsyncHelper.TryAsync(async () =>
        {
            var response = await SimpleHttp.DownloadAsString(serverEndpoint);
            var serverModel = JsonConvert.DeserializeObject<IndexViewModel>(response);
            var publicKey = new RSAParameters
            {
                Modulus = serverModel.Modulus.Base64ToBytes(),
                Exponent = serverModel.Exponent.Base64ToBytes()
            };
            services.AddSingleton(new GatewayLocator(serverEndpoint, publicKey));
        }, 5);
        services.AddLibraryDependencies();
        return services;
    }
}