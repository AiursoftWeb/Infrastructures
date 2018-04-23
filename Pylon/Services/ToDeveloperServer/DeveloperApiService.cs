using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer.ApiAddressModels;
using Aiursoft.Pylon.Models.Developer.ApiViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToDeveloperServer
{
    public class DeveloperApiService
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

        public async Task<AiurProtocal> IsValidAppAsync(string AppId, string AppSecret)
        {
            var url = new AiurUrl(_serviceLocation.Developer, "api", "IsValidApp", new IsValidateAppAddressModel
            {
                AppId = AppId,
                AppSecret = AppSecret
            });
            var result = await _http.Get(url);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return JResult;
        }

        public async Task<AppInfoViewModel> AppInfoAsync(string AppId)
        {
            var url = new AiurUrl(_serviceLocation.Developer, "api", "AppInfo", new AppInfoAddressModel
            {
                AppId = AppId
            });
            var result = await _http.Get(url);
            var JResult = JsonConvert.DeserializeObject<AppInfoViewModel>(result);

            //if (JResult.code != ErrorType.Success)
            //    throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
    }
}
