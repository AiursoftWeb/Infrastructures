using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToProbeServer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Views.Shared.Components.AiurUploader
{
    public class AiurUploader : ViewComponent
    {
        private readonly AppsContainer _appsContainer;
        private readonly TokenService _tokenService;

        public AiurUploader(
            AppsContainer appsContainer,
            TokenService tokenService)
        {
            _appsContainer = appsContainer;
            _tokenService = tokenService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
                string aspFor,
                string siteName,
                string path,
                int sizeInMb,
                string defaultFile,
                string allowedExtensions)
        {
            var accessToken = ViewBag.AccessToken as string ?? await _appsContainer.AccessToken();
            var token = await _tokenService.GetUploadTokenAsync(accessToken, siteName, "Upload", path);
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
