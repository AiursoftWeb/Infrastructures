using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;

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
