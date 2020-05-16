using Aiursoft.Archon.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;
using System.Security.Cryptography;

namespace Aiursoft.Archon.SDK.Services
{
    public class RSAService : IScopedDependency
    {
        private readonly PrivateKeyStore _privateKeyStore;
        private readonly ArchonLocator _archonLocator;
        private readonly RSA _rsa;

        public RSAService(
            PrivateKeyStore privateKeyStore,
            ArchonLocator archonLocator)
        {
            _rsa = RSA.Create();
            _privateKeyStore = privateKeyStore;
            _archonLocator = archonLocator;
        }

        public string SignData(string message)
        {
            var originalData = message.StringToBytes();
            _rsa.ImportParameters(_privateKeyStore.GetPrivateKey());
            var signedBytes = _rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return signedBytes.BytesToBase64();
        }

        public bool VerifyData(string originalMessage, string signedBase64)
        {
            var bytesToVerify = originalMessage.StringToBytes();
            var signedBytes = signedBase64.Base64ToBytes();
            _rsa.ImportParameters(_archonLocator.PublickKey);
            return _rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
