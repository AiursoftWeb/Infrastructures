using Aiursoft.Scanner.Abstract;
using System.Security.Cryptography;

namespace Aiursoft.Directory.Services;

public class PrivateKeyStore : ISingletonDependency
{
    private RSAParameters? _privateKey;

    public RSAParameters GetPrivateKey()
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