using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.HomeViewModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.Directory.SDK.Services;

public class ServerPublicKeyAccess : IScopedDependency
{
    private readonly ApiProxyService _proxy;
    private readonly CacheService _cacheService;
    private readonly DirectoryConfiguration _directoryConfiguration;
    
    public ServerPublicKeyAccess(
        ApiProxyService proxy,
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
            var response = await _proxy.Get(new AiurUrl(_directoryConfiguration.Instance), true);
            var serverModel = JsonConvert.DeserializeObject<DirectoryServerConfiguration>(response);
            var publicKey = new RSAParameters
            {
                Modulus = serverModel.Modulus.Base64ToBytes(),
                Exponent = serverModel.Exponent.Base64ToBytes()
            };
            return publicKey;
        });
    }
}
