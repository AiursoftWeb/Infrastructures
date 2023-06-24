using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.FoldersAddressModels;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.CSTools.Tools;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class FoldersService : IScopedDependency
{
    private readonly AiurProtocolClient  _http;
    private readonly ProbeConfiguration _serviceLocation;

    public FoldersService(
        AiurProtocolClient  http,
        IOptions<ProbeConfiguration> serviceLocation)
    {
        _http = http;
        _serviceLocation = serviceLocation.Value;
    }

    public async Task<AiurValue<Folder>> ViewContentAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance,
            $"/Folders/ViewContent/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new ViewContentAddressModel
            {
                AccessToken = accessToken
            });
        return await _http.Get<AiurValue<Folder>>(url);
    }

    public async Task<AiurResponse> CreateNewFolderAsync(string accessToken, string siteName, string folderNames,
        string newFolderName, bool recursiveCreate)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance,
            $"/Folders/CreateNewFolder/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurApiPayload( new CreateNewFolderAddressModel
        {
            AccessToken = accessToken,
            NewFolderName = newFolderName,
            RecursiveCreate = recursiveCreate
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurResponse> DeleteFolderAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance,
            $"/Folders/DeleteFolder/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurApiPayload( new DeleteFolderAddressModel
        {
            AccessToken = accessToken
        });
        return await _http.Post<AiurResponse>(url, form);
    }
}