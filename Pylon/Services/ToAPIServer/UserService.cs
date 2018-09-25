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
    public class UserService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public UserService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AiurProtocal> ChangeProfileAsync(string openId, string accessToken, string newNickName, int fileKey, string newBio)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "ChangeProfile", new ChangeProfileAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                NewNickName = newNickName,
                NewIconId = fileKey,
                NewBio = newBio
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurProtocal> ChangePasswordAsync(string openId, string accessToken, string oldPassword, string newPassword)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "ChangePassword", new ChangePasswordAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                OldPassword = oldPassword,
                NewPassword = newPassword
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);

            if (JResult.Code != ErrorType.Success && JResult.Code != ErrorType.WrongKey)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurValue<string>> ViewPhoneNumberAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "ViewPhoneNumber", new ViewPhoneNumberAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurProtocal> SetPhoneNumberAsync(string openId, string accessToken, string phoneNumber)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "SetPhoneNumber", new SetPhoneNumberAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                Phone = phoneNumber
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurCollection<AiurUserEmail>> ViewAllEmailsAsync(string accessToken, string openId)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "ViewAllEmails", new ViewAllEmailsAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AiurCollection<AiurUserEmail>>(result);
            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurProtocal> BindNewEmailAsync(string openId, string newEmail, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "BindNewEmail", new { });
            var form = new AiurUrl(string.Empty, new BindNewEmailAddressModel
            {
                OpenId = openId,
                NewEmail = newEmail,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return jResult;
        }

        public async Task<AiurProtocal> DeleteEmailAsync(string openId, string thatEmail, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "DeleteEmail", new { });
            var form = new AiurUrl(string.Empty, new DeleteEmailAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                ThatEmail = thatEmail
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return jResult;
        }

        public async Task<AiurProtocal> SendConfirmationEmailAsync(string accessToken, string userId, string email)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "SendConfirmationEmail", new { });
            var form = new AiurUrl(string.Empty, new SendConfirmationEmailAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId,
                Email = email
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return jresult;
        }

        public async Task<AiurCollection<Grant>> ViewGrantedAppsAsync(string accessToken, string userId)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "ViewGrantedApps", new ViewGrantedAppsAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<Grant>>(result);
            return jresult;
        }

        public async Task<AiurProtocal> DropGrantedAppsAsync(string accessToken, string userId, string appId)
        {
            var url = new AiurUrl(_serviceLocation.API, "User", "DropGrantedApps", new { });
            var form = new AiurUrl(string.Empty, new DropGrantedAppsAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId,
                AppIdToDrop = appId
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return jresult;
        }
    }
}
