using Aiursoft.Pylon.Services;
using System.Security.Cryptography;

namespace Aiursoft.Probe.Services
{
    public class PBRSAService
    {
        private readonly RSAParameters _privateKey;
        private readonly RSAParameters _publicKey;
        private readonly RSA _rsa;
        public PBRSAService(PBKeyPair keypair)
        {
            _privateKey = keypair.PrivateKey;
            _publicKey = keypair.PublicKey;
            _rsa = RSA.Create();
        }

        public string SignData(string message)
        {
            var originalData = message.StringToBytes();
            _rsa.ImportParameters(_privateKey);
            var signedBytes = _rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return signedBytes.BytesToBase64();
        }

        public bool VerifyData(string originalMessage, string signedBase64)
        {
            var bytesToVerify = originalMessage.StringToBytes();
            var signedBytes = signedBase64.Base64ToBytes();
            _rsa.ImportParameters(_publicKey);
            return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
