using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Aiursoft.Gateway.SDK.Models.API.HomeViewModels;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Gateway.SDK.Services;

public class GatewayLocator
{
    public readonly string Endpoint;
    private RSAParameters? _publicKey;

    public GatewayLocator(string endpoint)
    {
        Endpoint = endpoint;
    }

    public GatewayLocator(string endpoint, RSAParameters publicKey)
    {
        Endpoint = endpoint;
        _publicKey = publicKey;
    }

    public async Task<RSAParameters> GetPublicKey()
    {
        if (_publicKey == null)
        {
            var response = await SimpleHttp.DownloadAsString(Endpoint);
            var serverModel = JsonConvert.DeserializeObject<ArchonServerConfig>(response);
            _publicKey = new RSAParameters
            {
                Modulus = serverModel.Modulus.Base64ToBytes(),
                Exponent = serverModel.Exponent.Base64ToBytes()
            };
        }

        return _publicKey.Value;
    }
}