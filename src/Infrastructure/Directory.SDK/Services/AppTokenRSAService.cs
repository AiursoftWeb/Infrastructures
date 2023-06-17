using System.Security.Cryptography;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Directory.SDK.Services;

public class AppTokenRSAService : IScopedDependency
{
    private readonly RSA _rsa;
    private readonly ServerPublicKeyVault _serverPublicKeyVault;

    public AppTokenRSAService(ServerPublicKeyVault serverPublicKeyVault)
    {
        _rsa = RSA.Create();
        _serverPublicKeyVault = serverPublicKeyVault;
    }

    public async Task<bool> VerifyDataAsync(string originalMessage, string signedBase64)
    {
        var bytesToVerify = originalMessage.StringToBytes();
        var signedBytes = signedBase64.Base64ToBytes();
        _rsa.ImportParameters(await _serverPublicKeyVault.GetPublicKey());
        return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}