using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Archon;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToArchonServer
{
    public class ArchonApiService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public ArchonApiService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AccessTokenViewModel> AccessTokenAsync(string appId, string appSecret)
        {
            var url = new AiurUrl(_serviceLocation.Archon, "API", "AccessToken", new AccessTokenAddressModel
            {
                AppId = appId,
                AppSecret = appSecret
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AccessTokenViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(JResult);
            return JResult;
        }
    }
}
