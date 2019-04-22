using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.SiteAddressModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToProbeServer
{
    public class SitesService
    {
        private readonly HTTPService _http;
        private readonly ServiceLocation _serviceLocation;
        public SitesService(
            HTTPService http,
            ServiceLocation serviceLocation)
        {
            _http = http;
            _serviceLocation = serviceLocation;
        }

        public async Task<AiurProtocol> DeleteAppAsync(string accessToken, string appId)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Sites", "DeleteApp", new { });
            var form = new AiurUrl(string.Empty, new DeleteAppAddressModel
            {
                AccessToken = accessToken,
                AppId = appId
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
