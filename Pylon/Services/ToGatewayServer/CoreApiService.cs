using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.ApiViewModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToGatewayServer
{
    public class CoreApiService : IScopedDependency
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public CoreApiService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AllUserGrantedViewModel> AllUserGrantedAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "API", "AllUserGranted", new
            {
                accessToken
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AllUserGrantedViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurProtocol> DropGrantsAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "API", "DropGrants", new { });
            var form = new AiurUrl(string.Empty, new
            {
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
