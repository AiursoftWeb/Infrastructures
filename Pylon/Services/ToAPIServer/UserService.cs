using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models.API.ApiViewModels;
using Aiursoft.Pylon.Models.API.UserAddressModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToAPIServer
{
    public static class UserService
    {
        public async static Task<AiurProtocal> ChangeProfileAsync(string openId, string accessToken, string newNickName, string newIconAddress, string newBio)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "User", "ChangeProfile", new ChangeProfileAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                NewNickName = newNickName,
                NewIconAddress = newIconAddress,
                NewBio = newBio
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async static Task<AiurProtocal> ChangePasswordAsync(string openId, string accessToken, string oldPassword, string newPassword)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "User", "ChangePassword", new ChangePasswordAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                OldPassword = oldPassword,
                NewPassword = newPassword
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);

            if (JResult.Code != ErrorType.Success && JResult.Code != ErrorType.WrongKey)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async static Task<AiurValue<string>> ViewPhoneNumberAsync(string openId, string accessToken)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "User", "ViewPhoneNumber", new ViewPhoneNumberAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async static Task<AiurProtocal> SetPhoneNumberAsync(string penId, string accessToken, string phoneNumber)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "User", "SetPhoneNumber", new SetPhoneNumberAddressModel
            {
                AccessToken = accessToken,
                OpenId = penId,
                Phone = phoneNumber
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async static Task<AiurCollection<IUserEmail>> ViewAllEmailsAsync(string accessToken, string openId)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "User", "ViewAllEmails", new ViewAllEmailsAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AiurCollection<IUserEmail>>(result);
            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
    }
}
