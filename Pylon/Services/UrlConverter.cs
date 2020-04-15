using Aiursoft.Archon.SDK.Services;
using Aiursoft.Gateway.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Aiursoft.XelNaga.Models;
namespace Aiursoft.Pylon.Services
{
    public class UrlConverter : ITransientDependency
    {
        public readonly ServiceLocation _serviceLocation;
        private readonly AppsContainer _appsContainer;

        public UrlConverter(
            ServiceLocation serviceLocation,
            AppsContainer appsContainer)
        {
            _serviceLocation = serviceLocation;
            _appsContainer = appsContainer;
        }

        private AiurUrl GenerateAuthUrl(AiurUrl destination, string state, bool? justTry, bool register)
        {
            var action = register ? "register" : "authorize";
            var url = new AiurUrl(_serviceLocation.Gateway, "oauth", action, new AuthorizeAddressModel
            {
                AppId = _appsContainer._currentAppId,
                RedirectUri = destination.ToString(),
                State = state,
                TryAutho = justTry
            });
            return url;
        }

        public string UrlWithAuth(string serverRoot, string path, bool? justTry, bool register)
        {
            var localServer = new AiurUrl(serverRoot, "Auth", "AuthResult", new { });
            return GenerateAuthUrl(localServer, path, justTry, register).ToString();
        }
    }
}
