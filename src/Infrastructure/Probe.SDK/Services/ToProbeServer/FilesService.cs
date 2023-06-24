using System;
using System.IO;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models.FilesAddressModels;
using Aiursoft.Probe.SDK.Models.FilesViewModels;
using Aiursoft.Scanner.Abstraction;
using Aiursoft.CSTools.Tools;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class FilesService : IScopedDependency
{
    private readonly AiurProtocolClient _http;
    private readonly ProbeConfiguration _probeLocator;

    public FilesService(
        AiurProtocolClient http,
        IOptions<ProbeConfiguration> probeLocator)
    {
        _http = http;
        _probeLocator = probeLocator.Value;
    }

    public async Task<UploadFileViewModel> UploadFileAsync(string accessToken, string siteName, string folderNames,
        Stream file, bool recursiveCreate = false)
    {
        await Task.Delay(0);
        throw new NotImplementedException("Not implemented.");
    }

    public async Task<AiurResponse> DeleteFileAsync(string accessToken, string siteName, string folderNames)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance,
            $"/Files/DeleteFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurApiPayload(new DeleteFileAddressModel
        {
            AccessToken = accessToken
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<UploadFileViewModel> CopyFileAsync(string accessToken, string siteName, string folderNames,
        string targetSiteName, string targetFolderNames)
    {
        if (string.IsNullOrWhiteSpace(targetFolderNames))
        {
            targetFolderNames = "/";
        }

        var url = new AiurApiEndpoint(_probeLocator.Instance,
            $"/Files/CopyFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurApiPayload(new CopyFileAddressModel
        {
            AccessToken = accessToken,
            TargetSiteName = targetSiteName,
            TargetFolderNames = targetFolderNames
        });
        return await _http.Post<UploadFileViewModel>(url, form);
    }

    public async Task<UploadFileViewModel> RenameFileAsync(string accessToken, string siteName, string folderNames,
        string targetFileName)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance,
            $"/Files/RenameFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
        var form = new AiurApiPayload(new RenameFileAddressModel
        {
            AccessToken = accessToken,
            TargetFileName = targetFileName
        });
        return await _http.Post<UploadFileViewModel>(url, form);
    }
}