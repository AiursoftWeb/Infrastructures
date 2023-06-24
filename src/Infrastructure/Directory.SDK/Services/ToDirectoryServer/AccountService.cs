using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.AccountAddressModels;
using Aiursoft.Directory.SDK.Models.API.AccountViewModels;
using Aiursoft.Scanner.Abstractions;
using Microsoft.Extensions.Options;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Services.ToDirectoryServer;

public class AccountService : IScopedDependency
{
    private readonly DirectoryConfiguration _directoryConfiguration;
    private readonly AiurProtocolClient  _http;

    public AccountService(
        IOptions<DirectoryConfiguration> directoryConfiguration,
        AiurProtocolClient  http)
    {
        _directoryConfiguration = directoryConfiguration.Value;
        _http = http;
    }

    public async Task<CodeToOpenIdViewModel> CodeToOpenIdAsync(string accessToken, int code)
    {
        var url = new AiurApiEndpoint(_directoryConfiguration.Instance, "Account", "CodeToOpenId", new CodeToOpenIdAddressModel
        {
            AccessToken = accessToken,
            Code = code
        });
        return await _http.Get<CodeToOpenIdViewModel>(url);
    }

    public async Task<UserInfoViewModel> OpenIdToUserInfo(string accessToken, string openid)
    {
        var url = new AiurApiEndpoint(_directoryConfiguration.Instance, "Account", "UserInfo", new UserInfoAddressModel
        {
            AccessToken = accessToken,
            OpenId = openid
        });
        return await _http.Get<UserInfoViewModel>(url);
    }
}