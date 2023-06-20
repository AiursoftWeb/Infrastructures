using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Models.API.AppsAddressModels;
using Aiursoft.Directory.SDK.Models.API.AppsViewModels;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Services.ToDirectoryServer;

public class AppsService : IScopedDependency
{
    private readonly AiurProtocolClient _http;
    private readonly CacheService _cache;
    private readonly DirectoryConfiguration _directoryLocator;

    public AppsService(
        IOptions<DirectoryConfiguration> serviceLocation,
        AiurProtocolClient http,
        CacheService cache)
    {
        _directoryLocator = serviceLocation.Value;
        _http = http;
        _cache = cache;
    }

    public Task<AiurResponse> IsValidAppAsync(string appId, string appSecret)
    {
        return _cache.RunWithCache($"valid-app-with-appid-{appId}-secret-{appSecret}",
            async () =>
            {
                var url = new AiurApiEndpoint(_directoryLocator.Instance, "apps", "IsValidApp", new IsValidateAppAddressModel
                {
                    AppId = appId,
                    AppSecret = appSecret
                });
                return await _http.Get<AiurResponse>(url);
            });
    }

    public Task<AppInfoViewModel> AppInfoAsync(string appId)
    {
        return _cache.RunWithCache($"app-info-cache-{appId}", async () =>
        {
            var url = new AiurApiEndpoint(_directoryLocator.Instance, "apps", "AppInfo", new AppInfoAddressModel
            {
                AppId = appId
            });
            return await _http.Get<AppInfoViewModel>(url);
        });
    }

    /// <summary>
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="pageNumber">Starts from 1</param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async Task<AiurPagedCollection<Grant>> AllUserGrantedAsync(string accessToken, int pageNumber, int pageSize)
    {
        var url = new AiurApiEndpoint(_directoryLocator.Instance, "apps", "AllUserGranted", new AllUserGrantedAddressModel
        {
            AccessToken = accessToken,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return await _http.Get<AiurPagedCollection<Grant>>(url, true);
    }

    public async Task<AiurResponse> DropGrantsAsync(string accessToken)
    {
        var url = new AiurApiEndpoint(_directoryLocator.Instance, "apps", "DropGrants", new { });
        var form = new ApiPayload(new
        {
            AccessToken = accessToken
        });
        return await _http.Post<AiurResponse>(url, form);
    }
    
    public async Task<AccessTokenViewModel> AccessTokenAsync(string appId, string appSecret)
    {
        var url = new AiurApiEndpoint(_directoryLocator.Instance, "apps", "AccessToken", new AccessTokenAddressModel
        {
            AppId = appId,
            AppSecret = appSecret
        });
        return await _http.Get<AccessTokenViewModel>(url, true);
    }

    public async Task<IReadOnlyCollection<DirectoryApp>> GetAppsManagedByUser(string accessToken, string openId)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }
    
    public async Task<IReadOnlyCollection<DirectoryApp>> GetAllApps(string accessToken)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }

    /// <summary>
    /// This API can get any app's details.
    ///
    /// This API requires app with permissions: 'Trusted' and 'Manage other apps'.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<DirectoryApp> GetAppDetails(string accessToken, string appId)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }

    public async Task SetAppProperties(string accessToken, string appId)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }

    public async Task<string> CreateNewAppFor(
        string accessToken, 
        string name, 
        string description, 
        string iconPath, 
        string creatorId)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }
}