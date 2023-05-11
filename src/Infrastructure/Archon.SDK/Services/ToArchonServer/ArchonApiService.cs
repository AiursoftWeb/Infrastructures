using System.Threading.Tasks;
using Aiursoft.Archon.SDK.Models;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;

namespace Aiursoft.Archon.SDK.Services.ToArchonServer;

public class ArchonApiService : IScopedDependency
{
    private readonly ArchonLocator _archonLocator;
    private readonly APIProxyService _http;

    public ArchonApiService(
        ArchonLocator serviceLocation,
        APIProxyService http)
    {
        _archonLocator = serviceLocation;
        _http = http;
    }

    public async Task<AccessTokenViewModel> AccessTokenAsync(string appId, string appSecret)
    {
        var url = new AiurUrl(_archonLocator.Endpoint, "API", "AccessToken", new AccessTokenAddressModel
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