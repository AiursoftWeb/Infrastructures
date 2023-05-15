using Aiursoft.Gateway.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.Gateway.SDK.Services;

public class UrlConverter : ITransientDependency
{
    private readonly GatewayLocator _serviceLocation;

    public UrlConverter(
        GatewayLocator serviceLocation)
    {
        _serviceLocation = serviceLocation;
    }

    private AiurUrl GenerateAuthUrl(AiurUrl destination, string appId, string state, bool? justTry, bool register)
    {
        var action = register ? "register" : "authorize";
        var url = new AiurUrl(_serviceLocation.Endpoint, "oauth", action, new AuthorizeAddressModel
        {
            AppId = appId,
            RedirectUri = destination.ToString(),
            State = state,
            TryAutho = justTry
        });
        return url;
    }

    public string UrlWithAuth(string serverRoot, string appId, string path, bool? justTry, bool register)
    {
        var localServer = new AiurUrl(serverRoot, "Auth", "AuthResult", new { });
        return GenerateAuthUrl(localServer, appId, path, justTry, register).ToString();
    }
}