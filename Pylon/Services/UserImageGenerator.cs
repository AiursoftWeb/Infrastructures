using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class UserImageGenerator<T> where T : AiurUserBase
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly UserManager<T> _userManager;

        public UserImageGenerator(
            ServiceLocation serviceLocation,
            UserManager<T> userManager)
        {
            _serviceLocation = serviceLocation;
            _userManager = userManager;
        }

        public async Task<IHtmlContent> RenderUserImageAsync(ClaimsPrincipal user, int width = 20, int height = 20, string @class = "rounded")
        {
            var url = await GetUserImageUrl(user) + $"?w={width}&h={height}";
            var content = new HtmlContentBuilder();
            content.SetHtmlContent($"<img class='{@class}' src='{url}' style='width: {width}px; height: {height}px;' />");
            return content;
        }

        public async Task<string> GetUserImageUrl(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            return StorageService.GetProbeDownloadAddress(_serviceLocation, user.IconFilePath);
        }
    }
}
