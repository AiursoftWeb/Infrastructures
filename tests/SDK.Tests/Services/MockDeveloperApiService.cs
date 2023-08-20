using Aiursoft.AiurProtocol;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Services.ToDirectoryServer;
using Microsoft.Extensions.Options;

namespace Aiursoft.SDK.Tests.Services;

public class MockDeveloperApiService : AppsService
{
    public MockDeveloperApiService(
        IOptions<DirectoryConfiguration> serviceLocation,
        AiurProtocolClient http,
        CacheService cache) : base(serviceLocation, http, cache)
    {
    }

    public override Task<AiurResponse> IsValidAppAsync(string appId, string appSecret)
    {
        return Task.FromResult(new AiurResponse { Code = Code.NoActionTaken, Message = "Success!" });
    }
}