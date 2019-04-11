using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Aiursoft.Pylon.Services
{
    public class RSAService
    {
        private readonly RSAParameters _privateKey;
        public readonly RSAParameters _publicKey;
        public RSAService(AiurKeyPair keypair)
        {
            _privateKey = keypair.PrivateKey;
            _publicKey = keypair.PublicKey;
        }

        public string SignData(string message)
        {
            var originalData = message.StringToBytes();

            var rsa = RSA.Create();
            rsa.ImportParameters(_privateKey);
            
            var signedBytes = rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            
            return signedBytes.BytesToBase64();
        }

        public bool VerifyData(string originalMessage, string signedBase64)
        {
            var bytesToVerify = originalMessage.StringToBytes();
            var signedBytes = signedBase64.Base64ToBytes();

            var rsa = RSA.Create();
            rsa.ImportParameters(_publicKey);
            return rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
