using Aiursoft.Archon.SDK.Models;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;
using Newtonsoft.Json;
using System;

namespace Aiursoft.Archon.SDK.Services
{
    public class ACTokenValidator : IScopedDependency
    {
        private readonly RSAService _rsa;
        public ACTokenValidator(RSAService rsa)
        {
            _rsa = rsa;
        }

        public string ValidateAccessToken(string value)
        {
            ACToken token;
            try
            {
                var tokenParts = value.Split('.');
                string tokenBase64 = tokenParts[0], tokenSign = tokenParts[1];
                token = JsonConvert.DeserializeObject<ACToken>(tokenBase64.Base64ToString());
                if (DateTime.UtcNow > token.Expires)
                {
                    throw new AiurAPIModelException(ErrorType.Timeout, "Token was timed out!");
                }
                if (!_rsa.VerifyData(tokenBase64.Base64ToString(), tokenSign))
                {
                    throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid signature! Token could not be authorized!");
                }
            }
            catch
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Token was not in a valid format and can not be verified!");
            }
            return token.AppId;
        }
    }
}
