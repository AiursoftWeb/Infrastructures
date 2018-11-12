using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Aiursoft.Pylon.Services
{
    public class RSAService
    {
        private readonly RSAParameters _privateKey;
        private readonly RSAParameters _publicKey;
        public RSAService(AiurKeyPair keypair)
        {
            _privateKey = keypair.PrivateKey;
            _publicKey = keypair.PublicKey;
        }

        public string SignData(string message)
        {
            var originalData = message.StringToBytes();

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(_privateKey);

            var signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA256"));

            return signedBytes.BytesToBase64();
        }

        public bool VerifyData(string originalMessage, string signedBase64)
        {
            var bytesToVerify = originalMessage.StringToBytes();
            var signedBytes = signedBase64.Base64ToBytes();

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(_publicKey);

            return rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA256"), signedBytes);
        }
    }
}
