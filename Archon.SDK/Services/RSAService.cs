using Aiursoft.Archon.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Aiursoft.Archon.SDK.Services
{
    public class RSAService : IScopedDependency
    {
        private readonly AiurKeyPair _keypair;
        private readonly RSA _rsa;

        public RSAService(AiurKeyPair keypair)
        {
            _rsa = RSA.Create();
            _keypair = keypair;
        }

        public string SignData(string message)
        {
            var originalData = message.StringToBytes();
            _rsa.ImportParameters(_keypair.GetPrivateKey());
            var signedBytes = _rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return signedBytes.BytesToBase64();
        }

        public async Task<bool> VerifyData(string originalMessage, string signedBase64)
        {
            var bytesToVerify = originalMessage.StringToBytes();
            var signedBytes = signedBase64.Base64ToBytes();
            _rsa.ImportParameters(await _keypair.GetPublicKey());
            return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
