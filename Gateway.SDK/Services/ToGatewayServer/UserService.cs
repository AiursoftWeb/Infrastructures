using Aiursoft.Gateway.SDK.Models.API;
using Aiursoft.Gateway.SDK.Models.API.UserAddressModels;
using Aiursoft.Gateway.SDK.Models.API.UserViewModels;
using Aiursoft.Handler.Abstract.Exceptions;
using Aiursoft.Handler.Abstract.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.SDK.Services.ToGatewayServer
{
    public class UserService : IScopedDependency
    {
        private readonly GatewayLocator _serviceLocation;
        private readonly HTTPService _http;

        public UserService(
            GatewayLocator serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AiurProtocol> ChangeProfileAsync(string openId, string accessToken, string newNickName, string newIconFilePathName, string newBio)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ChangeProfile", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ChangePassword", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ViewPhoneNumber", new ViewPhoneNumberAddressModel
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "SetPhoneNumber", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ViewAllEmails", new ViewAllEmailsAddressModel
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "BindNewEmail", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "DeleteEmail", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "SendConfirmationEmail", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "SetPrimaryEmail", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ViewGrantedApps", new UserOperationAddressModel
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "DropGrantedApps", new { });
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userId"></param>
        /// <param name="pageNumber">Starts from 1.</param>
        /// <returns></returns>
        public async Task<AiurPagedCollection<AuditLog>> ViewAuditLogAsync(string accessToken, string userId, int pageNumber)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ViewAuditLog", new ViewAuditLogAddressModel
            {
                AccessToken = accessToken,
                OpenId = userId,
                PageNumber = pageNumber
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurPagedCollection<AuditLog>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<AiurThirdPartyAccount>> ViewSocialAccountsAsync(string accessToken, string userId)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ViewSocialAccounts", new UserOperationAddressModel
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "UnBindSocialAccount", new { });
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ViewHas2FAkey", new { });
            var form = new AiurUrl(string.Empty, new UserOperationAddressModel
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
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ViewTwoFactorEnabled", new { });
            var form = new AiurUrl(string.Empty, new UserOperationAddressModel
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

        public async Task<View2FAKeyViewModel> View2FAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "View2FAKey", new UserOperationAddressModel { });
            var form = new AiurUrl(string.Empty, new UserOperationAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<View2FAKeyViewModel>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<bool>> SetTwoFAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "SetTwoFAKey", new { });
            var form = new AiurUrl(string.Empty, new UserOperationAddressModel
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

        public async Task<AiurProtocol> ResetTwoFAKeyAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "ResetTwoFAKey", new { });
            var form = new AiurUrl(string.Empty, new UserOperationAddressModel
            {
                AccessToken = accessToken,
                OpenId = openId
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<bool>> TwoFAVerificyCodeAsync(string openId, string accessToken, string code)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "TwoFAVerificyCode", new TwoFAVerificyCodeAddressModel { });
            var form = new AiurUrl(string.Empty, new TwoFAVerificyCodeAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken,
                Code = code
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<bool>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurValue<bool>> DisableTwoFAAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "DisableTwoFA", new UserOperationAddressModel { });
            var form = new AiurUrl(string.Empty, new UserOperationAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<bool>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurCollection<string>> GetRecoveryCodesAsync(string openId, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "User", "GetRecoveryCodes", new UserOperationAddressModel { });
            var form = new AiurUrl(string.Empty, new UserOperationAddressModel
            {
                OpenId = openId,
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurCollection<string>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jresult);
            return jresult;
        }

    }
}
