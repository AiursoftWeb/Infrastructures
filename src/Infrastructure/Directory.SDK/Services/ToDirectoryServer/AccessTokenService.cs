using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models.API.APIAddressModels;
using Aiursoft.Directory.SDK.Models.API.APIViewModels;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Directory.SDK.Services.ToDirectoryServer;

public class AccessTokenService : IScopedDependency
{
    private readonly DirectoryConfiguration _directoryLocator;
    private readonly APIProxyService _http;

    public AccessTokenService(
        IOptions<DirectoryConfiguration> serviceLocation,
        APIProxyService http)
    {
        _directoryLocator = serviceLocation.Value;
        _http = http;
    }

    public async Task<AccessTokenViewModel> AccessTokenAsync(string appId, string appSecret)
    {
        var url = new AiurUrl(_directoryLocator.Instance, "API", "AccessToken", new AccessTokenAddressModel
        {
            AppId = appId,
            AppSecret = appSecret
        });
        var result = await _http.Get(url, true);
        var jResult = JsonConvert.DeserializeObject<AccessTokenViewModel>(result);

        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
}