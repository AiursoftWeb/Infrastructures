using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Newtonsoft.Json;

namespace Aiursoft.Probe.SDK.Services;

public class ProbeLocator
{
    public readonly string Endpoint;
    private ProbeServerConfig _config;

    public ProbeLocator(string endpoint)
    {
        Endpoint = endpoint;
    }

    public ProbeLocator(string endpoint, ProbeServerConfig config)
    {
        Endpoint = endpoint;
        _config = config;
    }

    public async Task<ProbeServerConfig> GetServerConfig()
    {
        if (_config == null)
        {
            var serverConfigString = await SimpleHttp.DownloadAsString(Endpoint);
            _config = JsonConvert.DeserializeObject<ProbeServerConfig>(serverConfigString);
        }

        return _config;
    }

    public Task<string> GetProbeOpenAddressAsync(string siteName, string[] folders, string fileName)
    {
        return GetProbeOpenAddressAsync(siteName, string.Join('/', folders), fileName);
    }

    public Task<string> GetProbeOpenAddressAsync(string siteName, string path, string fileName)
    {
        var fullPath = GetProbeFullPath(siteName, path, fileName);
        return GetProbeOpenAddressAsync(fullPath);
    }

    public Task<string> GetProbeDownloadAddressAsync(string siteName, string path, string fileName)
    {
        var fullPath = GetProbeFullPath(siteName, path, fileName);
        return GetProbeOpenAddressAsync(fullPath);
    }

    public Task<string> GetProbePlayerAddressAsync(string siteName, string path, string fileName)
    {
        var fullPath = GetProbeFullPath(siteName, path, fileName);
        return GetProbeOpenAddressAsync(fullPath);
    }

    public string GetProbeFullPath(string siteName, string path, string fileName)
    {
        var filePath = $"{path}/{fileName}".TrimStart('/');
        var fullPath = $"{siteName}/{filePath}".TrimStart('/');
        return fullPath;
    }

    public async Task<string> GetProbeOpenAddressAsync(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);

        var serverConfig = await this.GetServerConfig();
        var domain = string.Format(serverConfig.OpenPattern, siteName);
        var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
        return $"{domain}/{path}{fileName.ToUrlEncoded()}";
    }

    public async Task<string> GetProbeDownloadAddressAsync(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);
        var serverConfig = await this.GetServerConfig();
        var domain = string.Format(serverConfig.DownloadPattern, siteName);
        var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
        return $"{domain}/{path}{fileName.ToUrlEncoded()}";
    }

    public async Task<string> GetProbePlayerAddressAsync(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);
        var serverConfig = await this.GetServerConfig();
        var domain = string.Format(serverConfig.PlayerPattern, siteName);
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