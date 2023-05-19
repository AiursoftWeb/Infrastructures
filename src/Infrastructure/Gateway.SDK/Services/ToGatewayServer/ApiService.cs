using System.Threading.Tasks;
using Aiursoft.Gateway.SDK.Models.API.APIAddressModels;
using Aiursoft.Gateway.SDK.Models.API.APIViewModels;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.SDK.Services.ToGatewayServer;

public class ApiService : IScopedDependency
{
    private readonly GatewayLocator _gatewayLocator;
    private readonly APIProxyService _http;

    public ApiService(
        GatewayLocator serviceLocation,
        APIProxyService http)
    {
        _gatewayLocator = serviceLocation;
        _http = http;
    }

    public async Task<AccessTokenViewModel> AccessTokenAsync(string appId, string appSecret)
    {
        var url = new AiurUrl(_gatewayLocator.Endpoint, "API", "AccessToken", new AccessTokenAddressModel
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