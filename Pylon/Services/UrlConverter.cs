using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon;

namespace Aiursoft.Pylon.Services
{
    public class UrlConverter
    {
        public readonly ServiceLocation _serviceLocation;
        public UrlConverter(ServiceLocation serviceLocation)
        {
            _serviceLocation = serviceLocation;
        }

        private AiurUrl GenerateAuthUrl(AiurUrl destination, string state, bool? justTry, bool register)
        {
            var action = register ? "register" : "authorize";
            var url = new AiurUrl(_serviceLocation.API, "oauth", action, new AuthorizeAddressModel
            {
                appid = Extends.CurrentAppId,
                redirect_uri = destination.ToString(),
                response_type = "code",
                scope = "snsapi_base",
                state = state,
                tryAutho = justTry
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
