using System;
using System.Threading.Tasks;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models.TokenAddressModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class TokenService : IScopedDependency
{
    private readonly APIProxyService _http;
    private readonly ProbeConfiguration _serviceLocation;

    public TokenService(
        APIProxyService http,
        IOptions<ProbeConfiguration> serviceLocation)
    {
        _http = http;
        _serviceLocation = serviceLocation.Value;
    }

    /// <summary>
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="siteName"></param>
    /// <param name="permissions">Upload, Download</param>
    /// <param name="underPath"></param>
    /// <param name="lifespan"></param>
    /// <returns></returns>
    public async Task<string> GetTokenAsync(
        string accessToken,
        string siteName,
        string[] permissions,
        string underPath,
        TimeSpan lifespan)
    {
        var url = new AiurUrl(_serviceLocation.Endpoint, "Token", "GetToken", new { });
        var form = new AiurUrl(string.Empty, new GetTokenAddressModel
        {
            AccessToken = accessToken,
            SiteName = siteName,
            Permissions = string.Join(",", permissions),
            UnderPath = underPath,
            LifespanSeconds = (long)lifespan.TotalSeconds
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult.Value;
    }
}