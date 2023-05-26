using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.HomeViewModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Aiursoft.Directory.SDK.Services;

public class ServerPublicKeyVault : ISingletonDependency
{
    private readonly DirectoryConfiguration directoryConfiguration;
    private RSAParameters? _publicKey;

    public ServerPublicKeyVault(
        IOptions<DirectoryConfiguration> directoryConfiguration)
    {
        this.directoryConfiguration = directoryConfiguration.Value;
    }

    public async Task<RSAParameters> GetPublicKey()
    {
        if (_publicKey == null)
        {
            var response = await SimpleHttp.DownloadAsString(directoryConfiguration.Instance);
            var serverModel = JsonConvert.DeserializeObject<DirectoryServerConfiguration>(response);
            _publicKey = new RSAParameters
            {
                Modulus = serverModel.Modulus.Base64ToBytes(),
                Exponent = serverModel.Exponent.Base64ToBytes()
            };
        }

        return _publicKey.Value;
    }
}
