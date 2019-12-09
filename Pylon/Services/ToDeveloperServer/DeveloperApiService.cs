using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer.ApiAddressModels;
using Aiursoft.Pylon.Models.Developer.ApiViewModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToDeveloperServer
{
    public class DeveloperApiService : IScopedDependency
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;
        public DeveloperApiService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<bool> IsValidAppAsync(string appId, string appSecret)
        {
            var url = new AiurUrl(_serviceLocation.Developer, "api", "IsValidApp", new IsValidateAppAddressModel
            {
                AppId = appId,
                AppSecret = appSecret
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            return jresult.Code == ErrorType.Success;
        }

        public async Task<AppInfoViewModel> AppInfoAsync(string appId)
        {
            var url = new AiurUrl(_serviceLocation.Developer, "api", "AppInfo", new AppInfoAddressModel
            {
                AppId = appId
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AppInfoViewModel>(result);
            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
    }
}
