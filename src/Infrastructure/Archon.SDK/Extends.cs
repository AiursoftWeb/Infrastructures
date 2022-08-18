using System;
using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Scanner;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

                if (serverModel == null)
                {
                    throw new NullReferenceException($"Invalid json response: '{response}'!");
                }

                var publicKey = new RSAParameters
                {
                    Modulus = serverModel.Modulus?.Base64ToBytes() ?? throw new NullReferenceException("Modulus is null!"),
                    Exponent = serverModel.Exponent?.Base64ToBytes() ?? throw new NullReferenceException("Exponent is null!")
                };
                services.AddSingleton(new ArchonLocator(serverEndpoint, publicKey));
            }, 5);
            services.AddLibraryDependencies();
            return services;
        }
    }
}
