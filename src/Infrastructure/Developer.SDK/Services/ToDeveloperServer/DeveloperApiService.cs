using Aiursoft.Developer.SDK.Models.ApiAddressModels;
using Aiursoft.Developer.SDK.Models.ApiViewModels;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Developer.SDK.Services.ToDeveloperServer
{
    public class DeveloperApiService : IScopedDependency
    {
        private readonly DeveloperLocator _serviceLocation;
        private readonly APIProxyService _http;
        private readonly AiurCache _cache;

        public DeveloperApiService(
            DeveloperLocator serviceLocation,
            APIProxyService http,
            AiurCache cache)
        {
            _serviceLocation = serviceLocation;
            _http = http;
            _cache = cache;
        }

        public virtual Task<bool> IsValidAppAsync(string appId, string appSecret)
        {
            if (!new IsGuidOrEmpty().IsValid(appId))
            {
                return Task.FromResult(false);
            }
            if (!new IsGuidOrEmpty().IsValid(appSecret))
            {
                return Task.FromResult(false);
            }
            return _cache.GetAndCache($"ValidAppWithId-{appId}-Secret-{appSecret}", () => IsValidAppWithoutCacheAsync(appId, appSecret));
        }

        public Task<AppInfoViewModel> AppInfoAsync(string appId)
        {
            if (!new IsGuidOrEmpty().IsValid(appId))
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "Invalid app Id!");
            }
            return _cache.GetAndCache($"app-info-cache-{appId}", () => AppInfoWithoutCacheAsync(appId));
        }

        private async Task<bool> IsValidAppWithoutCacheAsync(string appId, string appSecret)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "api", "IsValidApp", new IsValidateAppAddressModel
            {
                AppId = appId,
                AppSecret = appSecret
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            return jResult != null && jResult.Code == ErrorType.Success;
        }

        private async Task<AppInfoViewModel> AppInfoWithoutCacheAsync(string appId)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "api", "AppInfo", new AppInfoAddressModel
            {
                AppId = appId
            });
            var json = await _http.Get(url, true);
            var result = JsonConvert.DeserializeObject<AppInfoViewModel>(json);
            if (result is not { Code: ErrorType.Success })
            {
                throw new AiurUnexpectedResponse(result);
            }
            return result;
        }
    }
}
