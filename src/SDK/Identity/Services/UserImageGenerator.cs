using System.Security.Claims;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Probe.SDK.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Identity.Services;

public class UserImageGenerator<T> where T : AiurUserBase
{
    private readonly ProbeSettingsFetcher _probeSettingsFetcher;
    private readonly UserManager<T> _userManager;

    public UserImageGenerator(
        ProbeSettingsFetcher probeSettingsFetcher,
        UserManager<T> userManager)
    {
        _probeSettingsFetcher = probeSettingsFetcher;
        _userManager = userManager;
    }

    public async Task<IHtmlContent> RenderUserImageAsync(
        ClaimsPrincipal user, 
        int width = 20, 
        int height = 20,
        string @class = "rounded")
    {
        var url = await GetUserImageUrl(user) + $"?w={width}&square=true";
        var content = new HtmlContentBuilder();
        content.SetHtmlContent($"<img class='{@class}' src='{url}' style='width: {width}px; height: {height}px;' />");
        return content;
    }

    public async Task<string> GetUserImageUrl(ClaimsPrincipal userClaims)
    {
        var user = await _userManager.GetUserAsync(userClaims);
        return await _probeSettingsFetcher.GetProbeOpenAddressAsync(user!.IconFilePath);
    }
}