using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.CSTools.Tools;
using Microsoft.Extensions.Options;
using Aiursoft.Canon;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Services;

namespace Aiursoft.Probe.SDK.Services;

public class ProbeSettingsFetcher : IScopedDependency
{
    private readonly CacheService _cacheService;
    private readonly AiurProtocolClient _AiurProtocolClient ;
    private readonly ProbeConfiguration _probeConfiguration;

    public ProbeSettingsFetcher(
        CacheService cacheService,
        AiurProtocolClient AiurProtocolClient ,
        IOptions<ProbeConfiguration> probeConfiguration)
    {
        _cacheService = cacheService;
        _AiurProtocolClient = AiurProtocolClient ;
        _probeConfiguration = probeConfiguration.Value;
    }

    public Task<ProbeDownloadPatternConfig> GetServerConfig()
    {
        return _cacheService.RunWithCache("probe-server-config", async () =>
        {
            return await _AiurProtocolClient.Get<ProbeDownloadPatternConfig>(new AiurApiEndpoint(_probeConfiguration.Instance, "/", new { }));
        });
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
        if (string.IsNullOrEmpty(fullPath))
        {
            throw new InvalidOperationException($"Can't get your file download address from empty full path!");
        }

        var paths = SplitStrings(fullPath);
        var fileName = paths.Last();
        var siteName = paths.First();
        var folders = paths.Take(paths.Length - 1).Skip(1).ToArray();
        return (siteName, folders, fileName);
    }

    private string[] SplitStrings(string folderNames)
    {
        return folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    }
}
