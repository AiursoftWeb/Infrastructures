using System;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models.TokenAddressModels;
using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class TokenService : IScopedDependency
{
    private readonly AiurProtocolClient _http;
    private readonly ProbeConfiguration _serviceLocation;

    public TokenService(
        AiurProtocolClient  http,
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
        var url = new AiurApiEndpoint(_serviceLocation.Instance, "Token", "GetToken", new { });
        var form = new AiurApiPayload(new GetTokenAddressModel
        {
            AccessToken = accessToken,
            SiteName = siteName,
            Permissions = string.Join(",", permissions),
            UnderPath = underPath,
            LifespanSeconds = (long)lifespan.TotalSeconds
        });
        var response = await _http.Post<AiurValue<string>>(url, form);
        return response.Value;
    }
}