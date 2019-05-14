using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.SitesAddressModels;
using Aiursoft.Pylon.Models.Probe.SitesViewModels;
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

        public async Task<AiurProtocol> CreateNewSiteAsync(string accessToken, string newSiteName)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Sites", "CreateNewSite", new { });
            var form = new AiurUrl(string.Empty, new CreateNewSiteAddressModel
            {
                AccessToken = accessToken,
                NewSiteName = newSiteName
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewMySitesViewModel> ViewMySitesAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Sites", "ViewMySites", new ViewMySitesAddressModel
            {
                AccessToken = accessToken
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<ViewMySitesViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> DeleteSiteAsync(string accessToken, string siteName)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Sites", "DeleteSite", new { });
            var form = new AiurUrl(string.Empty, new DeleteSiteAddressModel
            {
                AccessToken = accessToken,
                SiteName = siteName
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
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
