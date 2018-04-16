using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.ApiAddressModels;
using Aiursoft.Pylon.Models.API.ApiViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToAPIServer
{
    public class ApiService
    {
        public async static Task<ValidateAccessTokenViewModel> ValidateAccessTokenAsync(string AccessToken)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "api", "ValidateAccessToken", new
            {
                AccessToken = AccessToken
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<ValidateAccessTokenViewModel>(result);

            if (JResult.code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
        public async static Task<AccessTokenViewModel> AccessTokenAsync(string AppId, string AppSecret)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "API", "AccessToken", new AccessTokenAddressModel
            {
                AppId = AppId,
                AppSecret = AppSecret
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AccessTokenViewModel>(result);

            if (JResult.code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
        public async static Task<AllUserGrantedViewModel> AllUserGrantedAsync(string AccessToken)
        {
            var HTTPContainer = new HTTPService();
            var url = new AiurUrl(Values.ApiServerAddress, "API", "AllUserGranted", new
            {
                AccessToken = AccessToken
            });
            var result = await HTTPContainer.Get(url);
            var JResult = JsonConvert.DeserializeObject<AllUserGrantedViewModel>(result);

            if (JResult.code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
    }
}
