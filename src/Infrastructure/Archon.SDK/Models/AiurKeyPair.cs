using Aiursoft.Archon.SDK.Services.ToArchonServer;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Aiursoft.Archon.SDK.Models
{
    public class AiurKeyPair : ISingletonDependency
    {
        private RSAParameters? _publicKey;
        private RSAParameters? _privateKey;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScope;

        public AiurKeyPair(
            IConfiguration configuration,
            IServiceScopeFactory serviceScope)
        {
            _configuration = configuration;
            _serviceScope = serviceScope;
        }

        public async Task<RSAParameters> GetPublicKey()
        {
            if (_publicKey == null)
            {
                var scope = _serviceScope.CreateScope();
                var archonAPI = scope.ServiceProvider.GetService<ArchonApiService>();
                // Public key is from Archon API.
                var key = await archonAPI.GetKey();
                _publicKey = new RSAParameters
                {
                    Modulus = key.Modulus.Base64ToBytes(),
                    Exponent = key.Exponent.Base64ToBytes()
                };
            }
            return _publicKey.Value;
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
