using System;
using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Tools;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.SDK.Services;

public class AiursoftAppTokenValidator : IScopedDependency
{
    private readonly GatewayRSAService _gatewayRsa;

    public AiursoftAppTokenValidator(GatewayRSAService gatewayRsa)
    {
        _gatewayRsa = gatewayRsa;
    }

    public virtual string ValidateAccessToken(string value)
    {
        ACToken token;
        try
        {
            var tokenParts = value.Split('.');
            string tokenBase64 = tokenParts[0], tokenSign = tokenParts[1];
            token = JsonConvert.DeserializeObject<ACToken>(tokenBase64.Base64ToString());
            if (DateTime.UtcNow > token.Expires)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Token was timed out!");
            }

            if (!_gatewayRsa.VerifyData(tokenBase64.Base64ToString(), tokenSign))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized,
                    "Invalid signature! Token could not be authorized!");
            }
        }
        catch (Exception e)
        {
            throw new AiurAPIModelException(ErrorType.Unauthorized,
                $"Token was not in a valid format and can not be verified! Details: {e.Message}");
        }

        return token.AppId;
    }
}