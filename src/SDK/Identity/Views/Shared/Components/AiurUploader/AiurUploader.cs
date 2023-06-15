using System;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Identity.Views.Shared.Components.AiurUploader;

public class AiurUploader : ViewComponent
{
    private readonly CacheService _aiurCache;
    private readonly AppsContainer _appsContainer;
    private readonly SitesService _sitesService;
    private readonly TokenService _tokenService;

    public AiurUploader(
        AppsContainer appsContainer,
        TokenService tokenService,
        SitesService sitesService,
        CacheService aiurCache)
    {
        _appsContainer = appsContainer;
        _tokenService = tokenService;
        _sitesService = sitesService;
        _aiurCache = aiurCache;
    }

    private async Task<bool> OpenUpload(string siteName)
    {
        var accessToken = ViewBag.AccessToken as string ?? await _appsContainer.GetAccessTokenAsync();
        var site = await _sitesService.ViewSiteDetailAsync(accessToken, siteName);
        return site.Site.OpenToUpload;
    }

    private async Task<string> GetUploadToken(string siteName, string path)
    {
        if (await _aiurCache.RunWithCache($"site-public-status-{siteName}", () => OpenUpload(siteName)))
        {
            return string.Empty;
        }

        var accessToken = ViewBag.AccessToken as string ?? await _appsContainer.GetAccessTokenAsync();
        return await _tokenService.GetTokenAsync(accessToken, siteName, new[] { "Upload" }, path,
            TimeSpan.FromMinutes(100));
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