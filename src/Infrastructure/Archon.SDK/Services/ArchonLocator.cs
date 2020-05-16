using System.Security.Cryptography;

namespace Aiursoft.Archon.SDK.Services
{
    public class ArchonLocator
    {
        public ArchonLocator(string endpoint, RSAParameters publickKey)
        {
            Endpoint = endpoint;
            PublickKey = publickKey;
        }

        public string Endpoint { get; }
        public RSAParameters PublickKey { get; }
    }
}
