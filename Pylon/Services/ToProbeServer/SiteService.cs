using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.SitesAddressModels;
using Aiursoft.Pylon.Models.Probe.SitesViewModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToProbeServer
{
    public class SitesService : IScopedDependency
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

        public async Task<AiurProtocol> CreateNewSiteAsync(string accessToken, string newSiteName, bool openToUpload, bool openToDownload)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Sites", "CreateNewSite", new { });
            var form = new AiurUrl(string.Empty, new CreateNewSiteAddressModel
            {
                AccessToken = accessToken,
                NewSiteName = newSiteName,
                OpenToUpload = openToUpload,
                OpenToDownload = openToDownload
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

        public async Task<ViewSiteDetailViewModel> ViewSiteDetailAsync(string accessToken, string siteName)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Sites", "ViewSiteDetail", new ViewSiteDetailAddressModel
            {
                AccessToken = accessToken,
                SiteName = siteName
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<ViewSiteDetailViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> UpdateSiteInfoAsync(string accessToken, string oldSiteName, string newSiteName, bool openToUpload, bool openToDownload)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Sites", "UpdateSiteInfo", new { });
            var form = new AiurUrl(string.Empty, new UpdateSiteInfoAddressModel
            {
                AccessToken = accessToken,
                OldSiteName = oldSiteName,
                NewSiteName = newSiteName,
                OpenToDownload = openToDownload,
                OpenToUpload = openToUpload
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
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
