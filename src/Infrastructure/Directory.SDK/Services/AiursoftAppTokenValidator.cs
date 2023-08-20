using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.CSTools.Tools;
using Newtonsoft.Json;

namespace Aiursoft.Directory.SDK.Services;

public class AiursoftAppTokenValidator : IScopedDependency
{
    private readonly AppTokenRsaService _appTokenRsaService;

    public AiursoftAppTokenValidator(AppTokenRsaService appTokenRsaService)
    {
        _appTokenRsaService = appTokenRsaService;
    }

    public virtual async Task<string> ValidateAccessTokenAsync(string value)
    {
        AppToken token;
        try
        {
            var tokenParts = value.Split('.');
            string tokenBase64 = tokenParts[0], tokenSign = tokenParts[1];
            token = JsonConvert.DeserializeObject<AppToken>(tokenBase64.Base64ToString());
            if (DateTime.UtcNow > token.Expires)
            {
                throw new AiurServerException(Code.Unauthorized, "Token was timed out!");
            }

            if (!await _appTokenRsaService.VerifyDataAsync(tokenBase64.Base64ToString(), tokenSign))
            {
                throw new AiurServerException(Code.Unauthorized,
                    "Invalid signature! Token could not be authorized!");
            }
        }
        catch (Exception e)
        {
            throw new AiurServerException(Code.Unauthorized,
                $"Token was not in a valid format and can not be verified! Details: {e.Message}");
        }

        return token.AppId;
    }
}