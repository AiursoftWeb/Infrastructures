using System.Threading.Tasks;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Services.ToDirectoryServer;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;

namespace Aiursoft.SDK.Tests.Services;

public class MockDeveloperApiService : AppsService
{
    public MockDeveloperApiService(
        IOptions<DirectoryConfiguration> serviceLocation,
        AiurProtocolClient  http,
        CacheService cache) : base (serviceLocation, http, cache)
    {
    }
    
    public override Task<bool> IsValidAppAsync(string appId, string appSecret)
    {
        return Task.FromResult(true);
    }
}