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
        public static IServiceCollection AddArchonServer(this IServiceCollection services, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft archon server.
                serverEndpoint = "https://archon.aiursoft.com";
            }
            else
            {
                AsyncHelper.TryAsyncThreeTimes(async () =>
                {
                    var response = await new WebClient().DownloadStringTaskAsync(serverEndpoint);
                    var serverModel = JsonConvert.DeserializeObject<IndexViewModel>(response);
                    var publickKey = new RSAParameters
                    {
                        Modulus = serverModel.Modulus.Base64ToBytes(),
                        Exponent = serverModel.Exponent.Base64ToBytes()
                    };
                    services.AddSingleton(new ArchonLocator(serverEndpoint, publickKey));
                });
            }
            services.AddLibraryDependencies();
            return services;
        }
    }
}
