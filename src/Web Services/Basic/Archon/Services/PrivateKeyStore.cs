using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Aiursoft.Archon.Services
{
    public class PrivateKeyStore : ISingletonDependency
    {
        private RSAParameters? _privateKey;
        private readonly IConfiguration _configuration;

        public PrivateKeyStore(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public RSAParameters GetPrivateKey()
        {
            if (_privateKey == null)
            {
                // Private key is from configuration file.
                var _section = _configuration.GetSection("Key");
                _privateKey = new RSAParameters
                {
                    D = _section["D"].Base64ToBytes(),
                    DP = _section["DP"].Base64ToBytes(),
                    DQ = _section["DQ"].Base64ToBytes(),
                    Exponent = _section["Exponent"].Base64ToBytes(),
                    InverseQ = _section["InverseQ"].Base64ToBytes(),
                    Modulus = _section["Modulus"].Base64ToBytes(),
                    P = _section["P"].Base64ToBytes(),
                    Q = _section["Q"].Base64ToBytes()
                };
            }
            return _privateKey.Value;
        }
    }
}
