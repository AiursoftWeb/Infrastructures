﻿using Aiursoft.Directory.SDK.Models;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.CSTools.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aiursoft.Directory.Services;

public class TokenGenerator : IScopedDependency
{
    private readonly RSASignService _rsa;

    public TokenGenerator(RSASignService rsa)
    {
        _rsa = rsa;
    }

    public (string tokenString, DateTime expireTime) GenerateAccessToken(string appId)
    {
        var token = new AppToken
        {
            AppId = appId,
            Expires = DateTime.UtcNow + new TimeSpan(0, 20, 0)
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
}