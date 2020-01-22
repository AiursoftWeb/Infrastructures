using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Models.API.AccountAddressModels;
using Aiursoft.SDK.Models.API.AccountViewModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.SDK.Services.ToGatewayServer
{
    public class AccountService : IScopedDependency
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public AccountService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<CodeToOpenIdViewModel> CodeToOpenIdAsync(string accessToken, int code)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "Account", "CodeToOpenId", new CodeToOpenIdAddressModel
            {
                AccessToken = accessToken,
                Code = code
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<CodeToOpenIdViewModel>(result);

            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<UserInfoViewModel> OpenIdToUserInfo(string accessToken, string openid)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "Account", "UserInfo", new UserInfoAddressModel
            {
                AccessToken = accessToken,
                OpenId = openid
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<UserInfoViewModel>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }
    }
}
