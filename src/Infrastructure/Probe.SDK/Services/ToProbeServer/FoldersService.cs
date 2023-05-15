using System.Threading.Tasks;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.FoldersAddressModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class FoldersService : IScopedDependency
{
    private readonly APIProxyService _http;
    private readonly ProbeLocator _serviceLocation;

    public FoldersService(
        APIProxyService http,
        ProbeLocator serviceLocation)
    {
        _http = http;
        _serviceLocation = serviceLocation;
    }

    public async Task<AiurValue<Folder>> ViewContentAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurUrl(_serviceLocation.Endpoint,
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
        var url = new AiurUrl(_serviceLocation.Endpoint,
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
        var url = new AiurUrl(_serviceLocation.Endpoint,
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