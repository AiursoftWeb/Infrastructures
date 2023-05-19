using System.Security.Cryptography;

namespace Aiursoft.Gateway.SDK.Services;

public class GatewayLocator
{
    public GatewayLocator(string endpoint, RSAParameters publicKey)
    {
        Endpoint = endpoint;
        PublicKey = publicKey;
    }

    public string Endpoint { get; }
    public RSAParameters PublicKey { get; }
}