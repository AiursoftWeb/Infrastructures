using System.Security.Cryptography;

namespace Aiursoft.Archon.Services;

public class PrivateKeyStore
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