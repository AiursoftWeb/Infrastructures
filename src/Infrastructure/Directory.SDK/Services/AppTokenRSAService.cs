using System.Security.Cryptography;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.CSTools.Tools;

namespace Aiursoft.Directory.SDK.Services;

public class AppTokenRsaService : IScopedDependency
{
    private readonly RSA _rsa;
    private readonly ServerPublicKeyAccess _serverPublicKeyAccess;

    public AppTokenRsaService(ServerPublicKeyAccess serverPublicKeyAccess)
    {
        _rsa = RSA.Create();
        _serverPublicKeyAccess = serverPublicKeyAccess;
    }

    public async Task<bool> VerifyDataAsync(string originalMessage, string signedBase64)
    {
        var bytesToVerify = originalMessage.StringToBytes();
        var signedBytes = signedBase64.Base64ToBytes();
        _rsa.ImportParameters(await _serverPublicKeyAccess.GetPublicKey());
        return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}