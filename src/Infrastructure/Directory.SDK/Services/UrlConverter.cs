using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Scanner.Abstractions;
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

    private AiurApiEndpoint GenerateAuthUrl(AiurApiEndpoint destination, string appId, string state, bool? justTry, bool register)
    {
        var action = register ? "register" : "authorize";
        var url = new AiurApiEndpoint(_directoryConfiguration.Instance, "oauth", action, new AuthorizeAddressModel
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
        var localServer = new AiurApiEndpoint(serverRoot, "Auth", "AuthResult", new { });
        return GenerateAuthUrl(localServer, appId, path, justTry, register).ToString();
    }
}