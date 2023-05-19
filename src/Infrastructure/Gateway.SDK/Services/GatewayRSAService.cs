using System.Security.Cryptography;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Gateway.SDK.Services;

public class GatewayRSAService : IScopedDependency
{
    private readonly GatewayLocator _gatewayLocator;
    private readonly RSA _rsa;

    public GatewayRSAService(GatewayLocator gatewayLocator)
    {
        _rsa = RSA.Create();
        _gatewayLocator = gatewayLocator;
    }

    public async Task<bool> VerifyDataAsync(string originalMessage, string signedBase64)
    {
        var bytesToVerify = originalMessage.StringToBytes();
        var signedBytes = signedBase64.Base64ToBytes();
        _rsa.ImportParameters(await _gatewayLocator.GetPublicKey());
        return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}