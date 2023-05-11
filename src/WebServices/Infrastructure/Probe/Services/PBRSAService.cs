using System.Security.Cryptography;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Probe.Services;

public class PBRSAService : ITransientDependency
{
    private readonly PBKeyPair _keyPair;
    private readonly RSA _rsa;

    public PBRSAService(PBKeyPair keyPair)
    {
        _rsa = RSA.Create();
        _keyPair = keyPair;
    }

    public string SignData(string message)
    {
        var originalData = message.StringToBytes();
        _rsa.ImportParameters(_keyPair.GetKey());
        var signedBytes = _rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return signedBytes.BytesToBase64();
    }

    public bool VerifyData(string originalMessage, string signedBase64)
    {
        var bytesToVerify = originalMessage.StringToBytes();
        var signedBytes = signedBase64.Base64ToBytes();
        _rsa.ImportParameters(_keyPair.GetKey());
        return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}