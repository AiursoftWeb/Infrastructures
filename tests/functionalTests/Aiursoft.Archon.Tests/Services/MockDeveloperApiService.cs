using Aiursoft.Developer.SDK.Services;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Aiursoft.XelNaga.Services;
using System.Threading.Tasks;

namespace Aiursoft.Archon.Tests.Services
{
    public class MockDeveloperApiService : DeveloperApiService
    {
        public MockDeveloperApiService(
            DeveloperLocator serviceLocation, 
            HTTPService http, 
            AiurCache cache) : base(serviceLocation, http, cache)
        {
        }

        public override Task<bool> IsValidAppAsync(string appId, string appSecret)
        {
            return Task.FromResult(true);
        }
    }
}
