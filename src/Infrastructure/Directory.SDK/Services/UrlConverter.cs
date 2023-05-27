using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Microsoft.Extensions.Options;

namespace Aiursoft.Directory.SDK.Services;

public class UrlConverter : ITransientDependency
{
    private readonly DirectoryConfiguration _directoryConfiguration;

    public UrlConverter(
        IOptions<DirectoryConfiguration> directoryConfiguration)
    {
        _directoryConfiguration = directoryConfiguration.Value;
    }

    private AiurUrl GenerateAuthUrl(AiurUrl destination, string appId, string state, bool? justTry, bool register)
    {
        var action = register ? "register" : "authorize";
        var url = new AiurUrl(_directoryConfiguration.Instance, "oauth", action, new AuthorizeAddressModel
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