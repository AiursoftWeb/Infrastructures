using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.AccountAddressModels;
using Aiursoft.Directory.SDK.Models.API.AccountViewModels;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Directory.SDK.Services.ToDirectoryServer;

public class AccountService : IScopedDependency
{
    private readonly DirectoryConfiguration _directoryConfiguration;
    private readonly APIProxyService _http;

    public AccountService(
        IOptions<DirectoryConfiguration> directoryConfiguration,
        APIProxyService http)
    {
        _directoryConfiguration = directoryConfiguration.Value;
        _http = http;
    }

    public async Task<CodeToOpenIdViewModel> CodeToOpenIdAsync(string accessToken, int code)
    {
        var url = new AiurUrl(_directoryConfiguration.Instance, "Account", "CodeToOpenId", new CodeToOpenIdAddressModel
        {
            AccessToken = accessToken,
            Code = code
        });
        var result = await _http.Get(url, true);
        var jResult = JsonConvert.DeserializeObject<CodeToOpenIdViewModel>(result);

        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<UserInfoViewModel> OpenIdToUserInfo(string accessToken, string openid)
    {
        var url = new AiurUrl(_directoryConfiguration.Instance, "Account", "UserInfo", new UserInfoAddressModel
        {
            AccessToken = accessToken,
            OpenId = openid
        });
        var result = await _http.Get(url, true);
        var jResult = JsonConvert.DeserializeObject<UserInfoViewModel>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
}