using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.HomeViewModels;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.CSTools.Tools;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Services;

public class ServerPublicKeyAccess : IScopedDependency
{
    private readonly AiurProtocolClient _proxy;
    private readonly CacheService _cacheService;
    private readonly DirectoryConfiguration _directoryConfiguration;

    public ServerPublicKeyAccess(
        AiurProtocolClient proxy,
        CacheService cacheService,
        IOptions<DirectoryConfiguration> directoryConfiguration)
    {
        _proxy = proxy;
        _cacheService = cacheService;
        _directoryConfiguration = directoryConfiguration.Value;
    }

    public Task<RSAParameters> GetPublicKey()
    {
        return _cacheService.RunWithCache("server-public-key", async () =>
        {
            var response =
                await _proxy.Get<DirectoryServerConfiguration>(new AiurApiEndpoint(_directoryConfiguration.Instance,
                    "/", new { }));
            var publicKey = new RSAParameters
            {
                Modulus = response.Modulus.Base64ToBytes(),
                Exponent = response.Exponent.Base64ToBytes()
            };
            return publicKey;
        });
    }
}