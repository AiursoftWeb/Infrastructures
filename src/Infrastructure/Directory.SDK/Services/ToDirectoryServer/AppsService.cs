using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Models.API.AppsAddressModels;
using Aiursoft.Directory.SDK.Models.API.AppsViewModels;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Directory.SDK.Services.ToDirectoryServer;

public class AppsService : IScopedDependency
{
    private readonly ApiProxyService _http;
    private readonly CacheService _cache;
    private readonly DirectoryConfiguration _directoryLocator;

    public AppsService(
        IOptions<DirectoryConfiguration> serviceLocation,
        ApiProxyService http,
        CacheService cache)
    {
        _directoryLocator = serviceLocation.Value;
        _http = http;
        _cache = cache;
    }

    public virtual Task<bool> IsValidAppAsync(string appId, string appSecret)
    {
        if (!new IsGuidOrEmpty().IsValid(appId))
        {
            return Task.FromResult(false);
        }

        if (!new IsGuidOrEmpty().IsValid(appSecret))
        {
            return Task.FromResult(false);
        }

        return _cache.RunWithCache($"valid-app-with-appid-{appId}-secret-{appSecret}",
            async () =>
            {
                var url = new AiurUrl(_directoryLocator.Instance, "apps", "IsValidApp", new IsValidateAppAddressModel
                {
                    AppId = appId,
                    AppSecret = appSecret
                });
                var result = await _http.Get(url, true);
                var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
                return jResult.Code == ErrorType.Success;
            }, cacheCondition: result => result);
    }

    public Task<AppInfoViewModel> AppInfoAsync(string appId)
    {
        if (!new IsGuidOrEmpty().IsValid(appId))
        {
            throw new AiurAPIModelException(ErrorType.NotFound, "Invalid app Id!");
        }

        return _cache.RunWithCache($"app-info-cache-{appId}", async () =>
        {
            var url = new AiurUrl(_directoryLocator.Instance, "apps", "AppInfo", new AppInfoAddressModel
            {
                AppId = appId
            });
            var json = await _http.Get(url, true);
            var result = JsonConvert.DeserializeObject<AppInfoViewModel>(json);
            if (result.Code != ErrorType.Success)
            {
                throw new AiurUnexpectedResponse(result);
            }

            return result;
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
        var url = new AiurUrl(_directoryLocator.Instance, "apps", "AllUserGranted", new AllUserGrantedAddressModel
        {
            AccessToken = accessToken,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        var result = await _http.Get(url, true);
        var jResult = JsonConvert.DeserializeObject<AiurPagedCollection<Grant>>(result);

        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurProtocol> DropGrantsAsync(string accessToken)
    {
        var url = new AiurUrl(_directoryLocator.Instance, "apps", "DropGrants", new { });
        var form = new AiurUrl(string.Empty, new
        {
            AccessToken = accessToken
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
    
    public async Task<AccessTokenViewModel> AccessTokenAsync(string appId, string appSecret)
    {
        var url = new AiurUrl(_directoryLocator.Instance, "apps", "AccessToken", new AccessTokenAddressModel
        {
            AppId = appId,
            AppSecret = appSecret
        });
        var result = await _http.Get(url, true);
        var jResult = JsonConvert.DeserializeObject<AccessTokenViewModel>(result);

        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
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