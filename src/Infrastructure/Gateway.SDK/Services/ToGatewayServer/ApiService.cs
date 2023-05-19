using System.Threading.Tasks;
using Aiursoft.Archon.SDK.Models;
using Aiursoft.Gateway.SDK;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.SDK.Services.ToGatewayServer;

public class ApiService : IScopedDependency
{
    private readonly GatewayLocator _archonLocator;
    private readonly APIProxyService _http;

    public ApiService(
        GatewayLocator serviceLocation,
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