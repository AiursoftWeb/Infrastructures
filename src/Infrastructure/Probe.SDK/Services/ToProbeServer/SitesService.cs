using Aiursoft.AiurProtocol;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Models.SitesAddressModels;
using Aiursoft.Probe.SDK.Models.SitesViewModels;
using Aiursoft.Scanner.Abstractions;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer;

public class SitesService : IScopedDependency
{
    private readonly AiurProtocolClient  _http;
    private readonly ProbeConfiguration _probeLocator;

    public SitesService(
        AiurProtocolClient  http,
        IOptions<ProbeConfiguration> serviceLocation)
    {
        _http = http;
        _probeLocator = serviceLocation.Value;
    }

    public async Task<AiurResponse> CreateNewSiteAsync(string accessToken, string newSiteName, bool openToUpload,
        bool openToDownload)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance, "Sites", "CreateNewSite", new { });
        var form = new AiurApiPayload( new CreateNewSiteAddressModel
        {
            AccessToken = accessToken,
            NewSiteName = newSiteName,
            OpenToUpload = openToUpload,
            OpenToDownload = openToDownload
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<ViewMySitesViewModel> ViewMySitesAsync(string accessToken)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance, "Sites", "ViewMySites", new ViewMySitesAddressModel
        {
            AccessToken = accessToken
        });
        return await _http.Get<ViewMySitesViewModel>(url);
    }

    public async Task<ViewSiteDetailViewModel> ViewSiteDetailAsync(string accessToken, string siteName)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance, "Sites", "ViewSiteDetail", new ViewSiteDetailAddressModel
        {
            AccessToken = accessToken,
            SiteName = siteName
        });
        return await _http.Get<ViewSiteDetailViewModel>(url);
    }

    public async Task<AiurResponse> UpdateSiteInfoAsync(string accessToken, string oldSiteName, string newSiteName,
        bool openToUpload, bool openToDownload)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance, "Sites", "UpdateSiteInfo", new { });
        var form = new AiurApiPayload( new UpdateSiteInfoAddressModel
        {
            AccessToken = accessToken,
            OldSiteName = oldSiteName,
            NewSiteName = newSiteName,
            OpenToDownload = openToDownload,
            OpenToUpload = openToUpload
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurResponse> DeleteSiteAsync(string accessToken, string siteName)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance, "Sites", "DeleteSite", new { });
        var form = new AiurApiPayload( new DeleteSiteAddressModel
        {
            AccessToken = accessToken,
            SiteName = siteName
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<AiurResponse> DeleteAppAsync(string accessToken, string appId)
    {
        var url = new AiurApiEndpoint(_probeLocator.Instance, "Sites", "DeleteApp", new { });
        var form = new AiurApiPayload( new DeleteAppAddressModel
        {
            AccessToken = accessToken,
            AppId = appId
        });
        return await _http.Post<AiurResponse>(url, form);
    }
}