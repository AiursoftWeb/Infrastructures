using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Aiursoft.Probe.Services
{
    public class PBKeyPair : ISingletonDependency
    {
        public RSAParameters PublicKey { get; set; }
        public RSAParameters PrivateKey { get; set; }

        public PBKeyPair(IConfiguration configuration)
        {
            configuration = configuration.GetSection("PBKey");
            PublicKey = new RSAParameters
            {
                Exponent = configuration["Exponent"].Base64ToBytes(),
                Modulus = configuration["Modulus"].Base64ToBytes()
            };

            PrivateKey = new RSAParameters
            {
                D = configuration["D"].Base64ToBytes(),
                DP = configuration["DP"].Base64ToBytes(),
                DQ = configuration["DQ"].Base64ToBytes(),
                Exponent = configuration["Exponent"].Base64ToBytes(),
                InverseQ = configuration["InverseQ"].Base64ToBytes(),
                Modulus = configuration["Modulus"].Base64ToBytes(),
                P = configuration["P"].Base64ToBytes(),
                Q = configuration["Q"].Base64ToBytes()
            };
        }
    }
}
