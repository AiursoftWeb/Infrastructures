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
    public class ApiService
    {
        public async static Task<AiurProtocal> IsValidAppAsync(string AppId, string AppSecret)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.DeveloperServerAddress, "api", "IsValidApp", new IsValidateAppAddressModel
            {
                AppId = AppId,
                AppSecret = AppSecret
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return JResult;
        }
        public async static Task<AppInfoViewModel> AppInfoAsync(string AppId)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.DeveloperServerAddress, "api", "AppInfo", new AppInfoAddressModel
            {
                AppId = AppId
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AppInfoViewModel>(result);

            //if (JResult.code != ErrorType.Success)
            //    throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
    }
}
