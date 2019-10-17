﻿using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models.API.UserAddressModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToAPIServer
{
    public class UserService : IScopedDependency
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

        public async Task<AiurProtocol> ChangeProfileAsync(string openId, string accessToken, string newNickName, string newIconFilePathName, string newBio)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ChangeProfile", new ChangeProfileAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                NewNickName = newNickName,
                NewIconFilePathName = newIconFilePathName,
                NewBio = newBio
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);

            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> ChangePasswordAsync(string openId, string accessToken, string oldPassword, string newPassword)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ChangePassword", new ChangePasswordAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                OldPassword = oldPassword,
                NewPassword = newPassword
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);

            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<string>> ViewPhoneNumberAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewPhoneNumber", new ViewPhoneNumberAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> SetPhoneNumberAsync(string openId, string accessToken, string phoneNumber)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "SetPhoneNumber", new SetPhoneNumberAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                Phone = phoneNumber
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<AiurUserEmail>> ViewAllEmailsAsync(string accessToken, string openId)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewAllEmails", new ViewAllEmailsAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<AiurUserEmail>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> BindNewEmailAsync(string openId, string newEmail, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "BindNewEmail", new { });
            var form = new AiurUrl(string.Empty, new BindNewEmailAddressModel
            {
                OpenId = openId,
                NewEmail = newEmail,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> DeleteEmailAsync(string openId, string thatEmail, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "DeleteEmail", new { });
            var form = new AiurUrl(string.Empty, new DeleteEmailAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                ThatEmail = thatEmail
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> SendConfirmationEmailAsync(string accessToken, string userId, string email)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "SendConfirmationEmail", new { });
            var form = new AiurUrl(string.Empty, new SendConfirmationEmailAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId,
                Email = email
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> SetPrimaryEmailAsync(string accessToken, string userId, string email)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "SetPrimaryEmail", new { });
            var form = new AiurUrl(string.Empty, new SetPrimaryEmailAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId,
                Email = email
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<Grant>> ViewGrantedAppsAsync(string accessToken, string userId)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewGrantedApps", new UserOperationAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<Grant>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> DropGrantedAppsAsync(string accessToken, string userId, string appId)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "DropGrantedApps", new { });
            var form = new AiurUrl(string.Empty, new DropGrantedAppsAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId,
                AppIdToDrop = appId
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<AuditLog>> ViewAuditLogAsync(string accessToken, string userId)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewAuditLog", new UserOperationAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<AuditLog>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<SetTwoFAAddressModel>> ViewTwoFAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewTwoFAKey", new SetTwoFAAddressModel { });
            var form = new AiurUrl(string.Empty, new SetTwoFAAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<SetTwoFAAddressModel>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<SetTwoFAAddressModel>> SetTwoFAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "SetTwoFAKey", new SetTwoFAAddressModel { });
            var form = new AiurUrl(string.Empty, new SetTwoFAAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<SetTwoFAAddressModel>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<SetTwoFAAddressModel>> ResetTwoFAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ResetTwoFAKey", new SetTwoFAAddressModel { });
            var form = new AiurUrl(string.Empty, new SetTwoFAAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<SetTwoFAAddressModel>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<string>> TwoFAVerificyCodeAsync(string openId, string accessToken, string code)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "TwoFAVerificyCode", new SetTwoFAAddressModel { });
            var form = new AiurUrl(string.Empty, new SetTwoFAAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken,
                Code = code
            }) ;
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }
        public async Task<AiurValue<string>> DisableTwoFAAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "DisableTwoFA", new DisableTwoFAAddressModel { });
            var form = new AiurUrl(string.Empty, new DisableTwoFAAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<string>> RegenerateRecoveryCodesAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "RegenerateRecoveryCodes", new RegenerateRecoveryCodesAddressModel { });
            var form = new AiurUrl(string.Empty, new RegenerateRecoveryCodesAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

    }
}
