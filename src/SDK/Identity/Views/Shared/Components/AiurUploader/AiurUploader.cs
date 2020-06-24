using Aiursoft.Archon.SDK.Services;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Identity.Views.Shared.Components.AiurUploader
{
    public class AiurUploader : ViewComponent
    {
        private readonly AppsContainer _appsContainer;
        private readonly TokenService _tokenService;
        private readonly SitesService _sitesService;
        private readonly AiurCache _aiurCache;

        public AiurUploader(
            AppsContainer appsContainer,
            TokenService tokenService,
            SitesService sitesService,
            AiurCache aiurCache)
        {
            _appsContainer = appsContainer;
            _tokenService = tokenService;
            _sitesService = sitesService;
            _aiurCache = aiurCache;
        }

        private async Task<bool> OpenUpload(string siteName)
        {
            var accessToken = ViewBag.AccessToken as string ?? await _appsContainer.AccessToken();
            var site = await _sitesService.ViewSiteDetailAsync(accessToken, siteName);
            return site.Site.OpenToUpload;
        }

        private async Task<string> GetUploadToken(string siteName, string path)
        {
            if (!await _aiurCache.GetAndCache($"site-public-status-{siteName}", () => OpenUpload(siteName)))
            {
                var accessToken = ViewBag.AccessToken as string ?? await _appsContainer.AccessToken();
                return await _tokenService.GetTokenAsync(accessToken, siteName, new[] { "Upload" }, path);
            }
            return string.Empty;
        }

        public async Task<IViewComponentResult> InvokeAsync(
                string aspFor,
                string siteName,
                string path,
                int sizeInMb,
                string defaultFile,
                string allowedExtensions)
        {
            var token = await GetUploadToken(siteName, path);
            var model = new AiurUploaderViewModel
            {
                Name = aspFor,
                PBToken = token,
                Path = path,
                SiteName = siteName,
                SizeInMB = sizeInMb,
                DefaultFile = defaultFile,
                AllowedExtensions = allowedExtensions
            };
            return View(model);
        }
    }
}
