using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.SDK.Services;

public class ProbeSettingsFetcher : ISingletonDependency
{
    private ProbeDownloadPatternConfig _probeServerConfig;
    private readonly ProbeConfiguration _probeConfiguration;

    public ProbeSettingsFetcher(IOptions<ProbeConfiguration> probeConfiguration)
    {
        _probeConfiguration = probeConfiguration.Value;
    }

    public async Task<ProbeDownloadPatternConfig> GetServerConfig()
    {
        if (_probeServerConfig == null)
        {
            var serverConfigString = await SimpleHttp.DownloadAsString(_probeConfiguration.Endpoint);
            _probeServerConfig = JsonConvert.DeserializeObject<ProbeDownloadPatternConfig>(serverConfigString);
        }

        return _probeServerConfig;
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

        var serverConfig = await GetServerConfig();
        var domain = string.Format(serverConfig.OpenPattern, siteName);
        var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
        return $"{domain}/{path}{fileName.ToUrlEncoded()}";
    }

    public async Task<string> GetProbeDownloadAddressAsync(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);
        var serverConfig = await GetServerConfig();
        var domain = string.Format(serverConfig.DownloadPattern, siteName);
        var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
        return $"{domain}/{path}{fileName.ToUrlEncoded()}";
    }

    public async Task<string> GetProbePlayerAddressAsync(string fullPath)
    {
        var (siteName, folders, fileName) = SplitToPath(fullPath);
        var serverConfig = await GetServerConfig();
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
