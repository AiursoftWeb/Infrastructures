using System;
using System.Linq;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.Probe.SDK.Services;

public class ProbeLocator
{
    public string PlayerFormat;

    public ProbeLocator(
        string endpoint,
        string openFormat,
        string downloadFormat,
        string playerFormat)
    {
        Endpoint = endpoint;
        if (string.IsNullOrWhiteSpace(openFormat))
        {
            openFormat = endpoint + "/download/open/{0}";
        }

        OpenFormat = openFormat;
        if (string.IsNullOrWhiteSpace(downloadFormat))
        {
            downloadFormat = endpoint + "/download/file/{0}";
        }

        DownloadFormat = downloadFormat;
        if (string.IsNullOrWhiteSpace(playerFormat))
        {
            playerFormat = endpoint + "/Video/file/{0}";
        }

        PlayerFormat = playerFormat;
    }

    public string Endpoint { get; private set; }
    public string OpenFormat { get; }
    public string DownloadFormat { get; }

    public string GetProbeOpenAddress(string siteName, string[] folders, string fileName)
    {
        return GetProbeOpenAddress(siteName, string.Join('/', folders), fileName);
    }

    public string GetProbeOpenAddress(string siteName, string path, string fileName)
    {
        var fullPath = GetProbeFullPath(siteName, path, fileName);
        return GetProbeOpenAddress(fullPath);
    }

    public string GetProbeDownloadAddress(string siteName, string path, string fileName)
    {
        var fullPath = GetProbeFullPath(siteName, path, fileName);
        return GetProbeDownloadAddress(fullPath);
    }

    public string GetProbePlayerAddress(string siteName, string path, string fileName)
    {
        var fullPath = GetProbeFullPath(siteName, path, fileName);
        return GetProbePlayerAddress(fullPath);
    }

    public string GetProbeFullPath(string siteName, string path, string fileName)
    {
        var filePath = $"{path}/{fileName}".TrimStart('/');
        var fullPath = $"{siteName}/{filePath}".TrimStart('/');
        return fullPath;
    }

    public string GetProbeOpenAddress(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);
        var domain = string.Format(OpenFormat, siteName);
        var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
        return $"{domain}/{path}{fileName.ToUrlEncoded()}";
    }

    public string GetProbeDownloadAddress(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);
        var domain = string.Format(DownloadFormat, siteName);
        var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
        return $"{domain}/{path}{fileName.ToUrlEncoded()}";
    }

    public string GetProbePlayerAddress(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);
        var domain = string.Format(PlayerFormat, siteName);
        var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
        return $"{domain}/{path}{fileName.ToUrlEncoded()}";
    }

    private (string siteName, string[] folders, string fileName) SplitToPath(string fullPath)
    {
        if (fullPath == null || fullPath.Length == 0)
        {
            throw new AiurAPIModelException(ErrorType.NotFound,
                $"Can't get your file download address from path: '{fullPath}'!");
        }

        var paths = SplitStrings(fullPath);
        var fileName = paths.Last();
        var siteName = paths.First();
        var folders = paths.Take(paths.Count() - 1).Skip(1).ToArray();
        return (siteName, folders, fileName);
    }

    private string[] SplitStrings(string folderNames)
    {
        return folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
    }
}