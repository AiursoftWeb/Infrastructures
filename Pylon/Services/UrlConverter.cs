using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon;

namespace Aiursoft.Pylon.Services
{

    public static class UrlConverter
    {
        private static AiurUrl _GenerateAuthUrl(AiurUrl destination, string state, bool? justTry, bool register)
        {
            var action = register ? "register" : "authorize";
            var url = new AiurUrl(Values.ApiServerAddress, "oauth", action, new AuthorizeAddressModel
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

        public static string UrlWithAuth(string serverRoot, string path, bool? justTry, bool register)
        {
            return _GenerateAuthUrl(new AiurUrl(serverRoot, "Auth", "AuthResult", new { }), path, justTry, register).ToString();
        }
    }
}
