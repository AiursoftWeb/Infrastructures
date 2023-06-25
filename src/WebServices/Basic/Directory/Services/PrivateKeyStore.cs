using Aiursoft.Scanner.Abstractions;
using System.Security.Cryptography;

namespace Aiursoft.Directory.Services;

public class PrivateKeyStore : ISingletonDependency
{
    private RSAParameters? _privateKey;
    private object _lock = new object();

    public RSAParameters GetPrivateKey()
    {
        lock (_lock)
        {
            if (_privateKey == null)
            {
                var provider = new RSACryptoServiceProvider();
                _privateKey = provider.ExportParameters(true);
            }
        }
        return _privateKey.Value;
    }

    public RSAParameters GetPublicKey()
    {
        lock (_lock)
        {
            if (_privateKey == null)
            {
                var provider = new RSACryptoServiceProvider();
                _privateKey = provider.ExportParameters(true);
            }
        }

        var publicKey = new RSAParameters
        {
            Modulus = _privateKey.Value.Modulus,
            Exponent = _privateKey.Value.Exponent
        };
        return publicKey;
    }
}