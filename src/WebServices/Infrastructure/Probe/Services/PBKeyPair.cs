using System.Security.Cryptography;
using Aiursoft.Scanner.Abstract;

namespace Aiursoft.Probe.Services;

public class PBKeyPair : ISingletonDependency
{
    private RSAParameters? _privateKey;

    public RSAParameters GetKey()
    {
        if (_privateKey != null)
        {
            return _privateKey.Value;
        }

        var provider = new RSACryptoServiceProvider();
        _privateKey = provider.ExportParameters(true);
        return _privateKey.Value;
    }
}