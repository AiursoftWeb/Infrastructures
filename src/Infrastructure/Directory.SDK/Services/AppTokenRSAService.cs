using System.Security.Cryptography;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Directory.SDK.Services;

public class AppTokenRSAService : IScopedDependency
{
    private readonly RSA _rsa;
    private readonly ServerPublicKeyVault _keyVault;

    public AppTokenRSAService(ServerPublicKeyVault keyVault)
    {
        _rsa = RSA.Create();
        _keyVault = keyVault;
    }

    public async Task<bool> VerifyDataAsync(string originalMessage, string signedBase64)
    {
        var bytesToVerify = originalMessage.StringToBytes();
        var signedBytes = signedBase64.Base64ToBytes();
        _rsa.ImportParameters(await _keyVault.GetPublicKey());
        return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}