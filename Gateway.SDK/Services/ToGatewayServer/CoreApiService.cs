using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Gateway.SDK.Models.API;
using Aiursoft.Gateway.SDK.Models.API.APIAddressModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.SDK.Services.ToGatewayServer
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="pageNumber">Starts from 1</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<AiurPagedCollection<Grant>> AllUserGrantedAsync(string accessToken, int pageNumber, int pageSize)
        {
            var url = new AiurUrl(_serviceLocation.Gateway, "API", "AllUserGranted", new AllUserGrantedAddressModel
            {
                AccessToken = accessToken,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AiurPagedCollection<Grant>>(result);

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
