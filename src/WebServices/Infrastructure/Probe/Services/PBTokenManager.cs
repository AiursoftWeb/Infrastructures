using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Aiursoft.Probe.Services
{
    public class PBToken
    {
        public string SiteName { get; set; }
        public string UnderPath { get; set; }
        /// <summary>
        /// Upload, Download
        /// </summary>
        public string Permissions { get; set; }
        public DateTime Expires { get; set; }
    }

    public class PBTokenManager : ITransientDependency
    {
        private readonly PBRSAService _rsa;
        public PBTokenManager(PBRSAService rsa)
        {
            _rsa = rsa;
        }

        public (string, DateTime) GenerateAccessToken(string siteName, string underPath, string permissions, TimeSpan lifespan)
        {
            var token = new PBToken
            {
                SiteName = siteName,
                UnderPath = underPath,
                Permissions = permissions,
                Expires = DateTime.UtcNow + lifespan
            };
            var tokenJson = JsonConvert.SerializeObject(token, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            var tokenBase64 = tokenJson.StringToBase64();
            var tokenSign = _rsa.SignData(tokenJson);
            return ($"{tokenBase64}.{tokenSign}", token.Expires);
        }

        public PBToken ValidateAccessToken(string value)
        {
            PBToken token;
            try
            {
                var tokenParts = value.Split('.');
                string tokenBase64 = tokenParts[0];
                string tokenSign = tokenParts[1];
                token = JsonConvert.DeserializeObject<PBToken>(tokenBase64.Base64ToString());
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
            return token;
        }
    }
}
