using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon.Models.API.OAuthViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToAPIServer
{
    public class OAuthService
    {
        public async static Task<AiurValue<int>> PasswordAuthAsync(string appid, string email, string password)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "OAuth", "PasswordAuth", new { });
            var form = new AiurUrl(string.Empty, new PasswordAuthAddressModel
            {
                AppId = appid,
                Email = email,
                Password = password
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurValue<int>>(result);
            return jResult;
        }

        public async static Task<AiurProtocal> AppRegisterAsync(string email, string password, string confirmPassword)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "OAuth", "AppRegister", new { });
            var form = new AiurUrl(string.Empty, new AppRegisterAddressModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return jResult;
        }

        public async static Task<CodeToOpenIdViewModel> CodeToOpenIdAsync(int code, string AccessToken)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "OAuth", "CodeToOpenId", new CodeToOpenIdAddressModel
            {
                AccessToken = AccessToken,
                Code = code,
                grant_type = "authorization_code"
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<CodeToOpenIdViewModel>(result);

            if (JResult.code != ErrorType.Success)
                throw new Exception(JResult.message);
            return JResult;
        }

        public async static Task<UserInfoViewModel> OpenIdToUserInfo(string AccessToken, string openid)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "oauth", "UserInfo", new UserInfoAddressModel
            {
                access_token = AccessToken,
                openid = openid,
                lang = "en-US"
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<UserInfoViewModel>(result);
            if (JResult.code != ErrorType.Success)
                throw new Exception(JResult.message);
            return JResult;
        }
    }
}
