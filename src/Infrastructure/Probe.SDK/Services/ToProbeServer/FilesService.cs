using System.IO;
using System.Threading.Tasks;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models.FilesAddressModels;
using Aiursoft.Probe.SDK.Models.FilesViewModels;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class FilesService : IScopedDependency
{
    private readonly APIProxyService _http;
    private readonly ProbeLocator _serviceLocation;

    public FilesService(
        APIProxyService http,
        ProbeLocator serviceLocation)
    {
        _http = http;
        _serviceLocation = serviceLocation;
    }

    public async Task<UploadFileViewModel> UploadFileAsync(string accessToken, string siteName, string folderNames,
        Stream file, bool recursiveCreate = false)
    {
        var url = new AiurUrl(_serviceLocation.Endpoint,
            $"/Files/UploadFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new UploadFileAddressModel
            {
                Token = accessToken,
                RecursiveCreate = recursiveCreate
            });
        var result = await _http.PostWithFile(url, file, true);
        var jResult = JsonConvert.DeserializeObject<UploadFileViewModel>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurProtocol> DeleteFileAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurUrl(_serviceLocation.Endpoint,
            $"/Files/DeleteFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurUrl(string.Empty, new DeleteFileAddressModel
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

    public async Task<UploadFileViewModel> CopyFileAsync(string accessToken, string siteName, string folderNames,
        string targetSiteName, string targetFolderNames)
    {
        if (string.IsNullOrWhiteSpace(targetFolderNames))
        {
            targetFolderNames = "/";
        }

        var url = new AiurUrl(_serviceLocation.Endpoint,
            $"/Files/CopyFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurUrl(string.Empty, new CopyFileAddressModel
        {
            AccessToken = accessToken,
            TargetSiteName = targetSiteName,
            TargetFolderNames = targetFolderNames
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<UploadFileViewModel>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<UploadFileViewModel> RenameFileAsync(string accessToken, string siteName, string folderNames,
        string targetFileName)
    {
        var url = new AiurUrl(_serviceLocation.Endpoint,
            $"/Files/RenameFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurUrl(string.Empty, new RenameFileAddressModel
        {
            AccessToken = accessToken,
            TargetFileName = targetFileName
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<UploadFileViewModel>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
}