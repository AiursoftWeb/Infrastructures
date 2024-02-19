using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.CSTools.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aiursoft.Probe.Services;

// TODO: Unify the probe token with scoped directory token.
public class ProbeToken
{
    public string SiteName { get; init; }
    public string UnderPath { get; init; }

    /// <summary>
    ///     Upload, Download
    /// </summary>
    public string Permissions { get; init; }

    public DateTime Expires { get; init; }
}

public class ProbeTokenManager : ITransientDependency
{
    private readonly PBRSAService _rsa;

    public ProbeTokenManager(PBRSAService rsa)
    {
        _rsa = rsa;
    }

    public (string, DateTime) GenerateAccessToken(string siteName, string underPath, string permissions,
        TimeSpan lifespan)
    {
        var token = new ProbeToken
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

    public ProbeToken ValidateAccessToken(string value)
    {
        ProbeToken token;
        try
        {
            var tokenParts = value.Split('.');
            var tokenBase64 = tokenParts[0];
            var tokenSign = tokenParts[1];
            token = JsonConvert.DeserializeObject<ProbeToken>(tokenBase64.Base64ToString());
            if (DateTime.UtcNow > token.Expires)
            {
                throw new AiurServerException(Code.Timeout, "Token was timed out!");
            }

            if (!_rsa.VerifyData(tokenBase64.Base64ToString(), tokenSign))
            {
                throw new AiurServerException(Code.Unauthorized,
                    "Invalid signature! Token could not be authorized!");
            }
        }
        catch
        {
            throw new AiurServerException(Code.Unauthorized,
                "Token was not in a valid format and can not be verified!");
        }

        return token;
    }
}