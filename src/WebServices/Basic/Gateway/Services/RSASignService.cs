using System.Security.Cryptography;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Gateway.Services;

public class RSASignService : IScopedDependency
{
    private readonly PrivateKeyStore _privateKeyStore;
    private readonly RSA _rsa;

    public RSASignService(
        PrivateKeyStore privateKeyStore)
    {
        _rsa = RSA.Create();
        _privateKeyStore = privateKeyStore;
    }

    public string SignData(string message)
    {
        var originalData = message.StringToBytes();
        _rsa.ImportParameters(_privateKeyStore.GetPrivateKey());
        var signedBytes = _rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return signedBytes.BytesToBase64();
    }
}