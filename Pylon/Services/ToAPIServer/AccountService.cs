using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.AccountAddressModels;
using Aiursoft.Pylon.Models.API.AccountViewModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToAPIServer
{
    public class AccountService
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
            var url = new AiurUrl(_serviceLocation.API, "Account", "CodeToOpenId", new CodeToOpenIdAddressModel
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
            var url = new AiurUrl(_serviceLocation.API, "Account", "UserInfo", new UserInfoAddressModel
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
