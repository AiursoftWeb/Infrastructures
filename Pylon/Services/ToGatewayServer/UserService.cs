using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models.API.UserAddressModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToGatewayServer
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
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ChangeProfile", new { });
            var form = new AiurUrl(string.Empty, new ChangeProfileAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                NewNickName = newNickName,
                NewIconFilePathName = newIconFilePathName,
                NewBio = newBio
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);

            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> ChangePasswordAsync(string openId, string accessToken, string oldPassword, string newPassword)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ChangePassword", new { });
            var form = new AiurUrl(string.Empty, new ChangePasswordAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                OldPassword = oldPassword,
                NewPassword = newPassword
            });
            var result = await _http.Post(url, form, true);
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
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "SetPhoneNumber", new { });
            var form = new AiurUrl(string.Empty, new SetPhoneNumberAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId,
                Phone = phoneNumber
            });
            var result = await _http.Post(url, form, true);
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

        public async Task<AiurCollection<AiurThirdPartyAccount>> ViewSocialAccountsAsync(string accessToken, string userId)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewSocialAccounts", new UserOperationAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<AiurThirdPartyAccount>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> UnBindSocialAccountAsync(string accessToken, string userId, string providerName)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "UnBindSocialAccount", new { });
            var form = new AiurUrl(string.Empty, new UnBindSocialAccountAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId,
                ProviderName = providerName
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<bool>> ViewHas2FAkeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewHas2FAkey", new { });
            var form = new AiurUrl(string.Empty, new ViewHas2FAkeyAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<bool>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<bool>> ViewTwoFactorEnabledAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ViewTwoFactorEnabled", new { });
            var form = new AiurUrl(string.Empty, new ViewTwoFactorEnabledAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<bool>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<Get2FAKeyAddressModel>> View2FAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "View2FAKey", new Get2FAKeyAddressModel { });
            var form = new AiurUrl(string.Empty, new Get2FAKeyAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<Get2FAKeyAddressModel>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<bool>> SetTwoFAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "SetTwoFAKey", new { });
            var form = new AiurUrl(string.Empty, new Set2FAKeyAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<bool>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<bool>> ResetTwoFAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "User", "ResetTwoFAKey", new { });
            var form = new AiurUrl(string.Empty, new Reset2FAKeyAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<bool>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

    }
}
