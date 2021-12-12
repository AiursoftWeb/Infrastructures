using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Scanner;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;

namespace Aiursoft.Archon.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddArchonServer(this IServiceCollection services, string serverEndpoint)
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
                services.AddSingleton(new ArchonLocator(serverEndpoint, publicKey));
            }, 5);
            services.AddLibraryDependencies();
            return services;
        }
    }
}
