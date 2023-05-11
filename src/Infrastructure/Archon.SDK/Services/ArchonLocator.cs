using System.Security.Cryptography;

namespace Aiursoft.Archon.SDK.Services;

public class ArchonLocator
{
    public ArchonLocator(string endpoint, RSAParameters publicKey)
    {
        Endpoint = endpoint;
        PublicKey = publicKey;
    }

    public string Endpoint { get; }
    public RSAParameters PublicKey { get; }
}