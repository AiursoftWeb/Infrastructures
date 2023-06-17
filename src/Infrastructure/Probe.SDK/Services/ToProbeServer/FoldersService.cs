using System.Threading.Tasks;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.FoldersAddressModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class FoldersService : IScopedDependency
{
    private readonly ApiProxyService _http;
    private readonly ProbeConfiguration _serviceLocation;

    public FoldersService(
        ApiProxyService http,
        IOptions<ProbeConfiguration> serviceLocation)
    {
        _http = http;
        _serviceLocation = serviceLocation.Value;
    }

    public async Task<AiurValue<Folder>> ViewContentAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurUrl(_serviceLocation.Instance,
            $"/Folders/ViewContent/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new ViewContentAddressModel
            {
                AccessToken = accessToken
            });
        var result = await _http.Get(url, true);
        var jResult = JsonConvert.DeserializeObject<AiurValue<Folder>>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurProtocol> CreateNewFolderAsync(string accessToken, string siteName, string folderNames,
        string newFolderName, bool recursiveCreate)
    {
        var url = new AiurUrl(_serviceLocation.Instance,
            $"/Folders/CreateNewFolder/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurUrl(string.Empty, new CreateNewFolderAddressModel
        {
            AccessToken = accessToken,
            NewFolderName = newFolderName,
            RecursiveCreate = recursiveCreate
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurProtocol> DeleteFolderAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurUrl(_serviceLocation.Instance,
            $"/Folders/DeleteFolder/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurUrl(string.Empty, new DeleteFolderAddressModel
        {
            AccessToken = accessToken
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
}