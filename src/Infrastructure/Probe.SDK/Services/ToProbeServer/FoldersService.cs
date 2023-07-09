using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.FoldersAddressModels;
using Aiursoft.Scanner.Abstractions;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class FoldersService : IScopedDependency
{
    private readonly AiurProtocolClient _http;
    private readonly ProbeConfiguration _serviceLocation;

    public FoldersService(
        AiurProtocolClient http,
        IOptions<ProbeConfiguration> serviceLocation)
    {
        _http = http;
        _serviceLocation = serviceLocation.Value;
    }

    public async Task<AiurValue<Folder>> ViewContentAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance,
            "/Folders/ViewContent/{SiteName}/{**FolderNames}", new ViewContentAddressModel
            {
                SiteName = siteName,
                FolderNames = folderNames,
                AccessToken = accessToken
            });
        return await _http.Get<AiurValue<Folder>>(url);
    }

    public async Task<AiurResponse> CreateNewFolderAsync(string accessToken, string siteName, string folderNames,
        string newFolderName, bool recursiveCreate)
    {
        var url = new AiurApiEndpoint(_serviceLocation.Instance,
            "/Folders/CreateNewFolder/{SiteName}/{FolderNames}", new CreateNewFolderAddressModel
            {
                SiteName = siteName,
                FolderNames = folderNames
            });
        var form = new AiurApiPayload(new CreateNewFolderFormModel
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
            "/Folders/DeleteFolder/{SiteName}/{FolderNames}", new DeleteFolderAddressModel
            {
                SiteName = siteName,
                FolderNames = folderNames
            });
        var form = new AiurApiPayload(new DeleteFolderFormModel
        {
            AccessToken = accessToken
        });
        return await _http.Post<AiurResponse>(url, form);
    }
}