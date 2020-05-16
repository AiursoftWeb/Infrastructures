using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Scanner;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace Aiursoft.Archon.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddArchonServer(this IServiceCollection services, bool loadArchonConfig, string serverEndpoint = null)
        {
            if (string.IsNullOrWhiteSpace(serverEndpoint))
            {
                // Default Aiursoft archon server.
                serverEndpoint = "https://archon.aiursoft.com";
            }
            var entryName = Assembly.GetEntryAssembly().GetName().Name;
            var exectName = Assembly.GetExecutingAssembly().GetName().Name;
            if (exectName.StartsWith(entryName) || !loadArchonConfig)
            {
                services.AddSingleton(new ArchonLocator(serverEndpoint));
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
