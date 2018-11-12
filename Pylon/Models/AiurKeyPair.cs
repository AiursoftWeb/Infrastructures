using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Aiursoft.Pylon.Models
{
    public class AiurKeyPair
    {
        public RSAParameters PublicKey { get; set; }
        public RSAParameters PrivateKey { get; set; }
    }
}
