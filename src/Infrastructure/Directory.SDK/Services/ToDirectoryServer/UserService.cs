using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Models.API.UserAddressModels;
using Aiursoft.Directory.SDK.Models.API.UserViewModels;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Services;

namespace Aiursoft.Directory.SDK.Services.ToDirectoryServer;

public class UserService : IScopedDependency
{
    private readonly AiurProtocolClient _http;
    private readonly DirectoryConfiguration _serviceLocation;

    public UserService(
        IOptions<DirectoryConfiguration> serviceLocation,
        AiurProtocolClient http)
    {
        _serviceLocation = serviceLocation.Value;
        _http = http;
    }

    public async Task<AiurResponse> ChangeProfileAsync(string openId, string accessToken, string newNickName,
        string newIconFilePathName, string newBio)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ChangeProfile", new { });
        var form = new ApiPayload(new ChangeProfileAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId,
            NewNickName = newNickName,
            NewIconFilePathName = newIconFilePathName,
            NewBio = newBio
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurResponse> ChangePasswordAsync(string openId, string accessToken, string oldPassword,
        string newPassword)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ChangePassword", new { });
        var form = new ApiPayload(new ChangePasswordAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId,
            OldPassword = oldPassword,
            NewPassword = newPassword
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurValue<string>> ViewPhoneNumberAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ViewPhoneNumber", new ViewPhoneNumberAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId
        });
        return await _http.Get<AiurValue<string>>(url, true);
    }

    public async Task<AiurResponse> SetPhoneNumberAsync(string openId, string accessToken, string phoneNumber)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "SetPhoneNumber", new { });
        var form = new ApiPayload(new SetPhoneNumberAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId,
            Phone = phoneNumber
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurCollection<AiurUserEmail>> ViewAllEmailsAsync(string accessToken, string openId)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ViewAllEmails", new ViewAllEmailsAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId
        });
        return await _http.Get<AiurCollection<AiurUserEmail>>(url, true);
    }

    public async Task<AiurResponse> BindNewEmailAsync(string openId, string newEmail, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "BindNewEmail", new { });
        var form = new ApiPayload(new BindNewEmailAddressModel
        {
            OpenId = openId,
            NewEmail = newEmail,
            AccessToken = accessToken
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurResponse> DeleteEmailAsync(string openId, string thatEmail, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "DeleteEmail", new { });
        var form = new ApiPayload(new DeleteEmailAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId,
            ThatEmail = thatEmail
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurResponse> SendConfirmationEmailAsync(string accessToken, string userId, string email)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "SendConfirmationEmail", new { });
        var form = new ApiPayload(new SendConfirmationEmailAddressModel
        {
            AccessToken = accessToken,
            OpenId = userId,
            Email = email
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurResponse> SetPrimaryEmailAsync(string accessToken, string userId, string email)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "SetPrimaryEmail", new { });
        var form = new ApiPayload(new SetPrimaryEmailAddressModel
        {
            AccessToken = accessToken,
            OpenId = userId,
            Email = email
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurCollection<Grant>> ViewGrantedAppsAsync(string accessToken, string userId)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ViewGrantedApps", new UserOperationAddressModel
        {
            AccessToken = accessToken,
            OpenId = userId
        });
        return await _http.Get<AiurCollection<Grant>>(url);
    }

    public async Task<AiurResponse> DropGrantedAppsAsync(string accessToken, string userId, string appId)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "DropGrantedApps", new { });
        var form = new ApiPayload(new DropGrantedAppsAddressModel
        {
            AccessToken = accessToken,
            OpenId = userId,
            AppIdToDrop = appId
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    /// <summary>
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="userId"></param>
    /// <param name="pageNumber">Starts from 1.</param>
    /// <returns></returns>
    public async Task<AiurPagedCollection<AuditLog>> ViewAuditLogAsync(string accessToken, string userId,
        int pageNumber)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ViewAuditLog", new ViewAuditLogAddressModel
        {
            AccessToken = accessToken,
            OpenId = userId,
            PageNumber = pageNumber
        });
        return await _http.Get<AiurPagedCollection<AuditLog>>(url);
    }

    public async Task<AiurCollection<AiurThirdPartyAccount>> ViewSocialAccountsAsync(string accessToken, string userId)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ViewSocialAccounts", new UserOperationAddressModel
        {
            AccessToken = accessToken,
            OpenId = userId
        });
        return await _http.Get<AiurCollection<AiurThirdPartyAccount>>(url);
    }

    public async Task<AiurResponse> UnBindSocialAccountAsync(string accessToken, string userId, string providerName)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "UnBindSocialAccount", new { });
        var form = new ApiPayload(new UnBindSocialAccountAddressModel
        {
            AccessToken = accessToken,
            OpenId = userId,
            ProviderName = providerName
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurValue<bool>> ViewHas2FAKeyAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ViewHas2FAKey", new { });
        var form = new ApiPayload(new UserOperationAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId
        });
        return await _http.Post<AiurValue<bool>>(url, form);
    }

    public async Task<AiurValue<bool>> ViewTwoFactorEnabledAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ViewTwoFactorEnabled", new { });
        var form = new ApiPayload(new UserOperationAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId
        });
        return await _http.Post<AiurValue<bool>>(url, form);
    }

    public async Task<View2FAKeyViewModel> View2FAKeyAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "View2FAKey", new UserOperationAddressModel());
        var form = new ApiPayload(new UserOperationAddressModel
        {
            OpenId = openId,
            AccessToken = accessToken
        });
        return await _http.Post<View2FAKeyViewModel>(url, form);
    }

    public async Task<AiurValue<bool>> SetTwoFAKeyAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "SetTwoFAKey", new { });
        var form = new ApiPayload(new UserOperationAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId
        });
        return await _http.Post<AiurValue<bool>>(url, form);
    }

    public async Task<AiurResponse> ResetTwoFAKeyAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "ResetTwoFAKey", new { });
        var form = new ApiPayload(new UserOperationAddressModel
        {
            AccessToken = accessToken,
            OpenId = openId
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurValue<bool>> TwoFAVerifyCodeAsync(string openId, string accessToken, string code)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "TwoFAVerifyCode", new TwoFAVerifyCodeAddressModel());
        var form = new ApiPayload(new TwoFAVerifyCodeAddressModel
        {
            OpenId = openId,
            AccessToken = accessToken,
            Code = code
        });
        return await _http.Post<AiurValue<bool>>(url, form);
    }

    public async Task<AiurValue<bool>> DisableTwoFAAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "DisableTwoFA", new UserOperationAddressModel());
        var form = new ApiPayload(new UserOperationAddressModel
        {
            OpenId = openId,
            AccessToken = accessToken
        });
        return await _http.Post<AiurValue<bool>>(url, form);
    }

    public async Task<AiurCollection<string>> GetRecoveryCodesAsync(string openId, string accessToken)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "User", "GetRecoveryCodes", new UserOperationAddressModel());
        var form = new ApiPayload(new UserOperationAddressModel
        {
            OpenId = openId,
            AccessToken = accessToken
        });
        return await _http.Post<AiurCollection<string>>(url, form);
    }
}