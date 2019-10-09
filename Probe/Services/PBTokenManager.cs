using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
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

        public (string, DateTime) GenerateAccessToken(string siteName, string underPath, string permissions)
        {
            var token = new PBToken
            {
                SiteName = siteName,
                UnderPath = underPath,
                Permissions = permissions,
                Expires = DateTime.UtcNow + new TimeSpan(0, 60, 0)
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
                var tokenparts = value.Split('.');
                string tokenBase64 = tokenparts[0];
                string tokenSign = tokenparts[1];
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
