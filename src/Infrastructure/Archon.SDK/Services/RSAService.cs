using System.Security.Cryptography;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Archon.SDK.Services;

public class RSAService : IScopedDependency
{
    private readonly ArchonLocator _archonLocator;
    private readonly RSA _rsa;

    public RSAService(ArchonLocator archonLocator)
    {
        _rsa = RSA.Create();
        _archonLocator = archonLocator;
    }

    public bool VerifyData(string originalMessage, string signedBase64)
    {
        var bytesToVerify = originalMessage.StringToBytes();
        var signedBytes = signedBase64.Base64ToBytes();
        _rsa.ImportParameters(_archonLocator.PublicKey);
        return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}