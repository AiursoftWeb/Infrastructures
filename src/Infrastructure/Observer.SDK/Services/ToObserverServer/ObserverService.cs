using System;
using System.Threading.Tasks;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.SDK.Configuration;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Models.EventAddressModels;
using Aiursoft.Observer.SDK.Models.EventViewModels;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Observer.SDK.Services.ToObserverServer;

public class ObserverService : IScopedDependency
{
    private readonly ApiProxyService _http;
    private readonly ObserverConfiguration _observerLocator;

    public ObserverService(
        ApiProxyService http,
        IOptions<ObserverConfiguration> observerLocator)
    {
        _http = http;
        _observerLocator = observerLocator.Value;
    }

    public Task<AiurProtocol> LogExceptionAsync(string accessToken, Exception e, string httpRequestPath = "Inline")
    {
        return LogAsync(accessToken, e.Message, e.StackTrace, EventLevel.Exception, httpRequestPath);
    }

    public async Task<AiurProtocol> LogAsync(string accessToken, string message, string stackTrace,
        EventLevel eventLevel, string path)
    {
        var url = new AiurUrl(_observerLocator.Instance, "Event", "Log", new { });
        var form = new AiurUrl(string.Empty, new LogAddressModel
        {
            AccessToken = accessToken,
            Message = message,
            StackTrace = stackTrace,
            EventLevel = eventLevel,
            Path = path
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<ViewLogViewModel> ViewAsync(string accessToken)
    {
        var url = new AiurUrl(_observerLocator.Instance, "Event", "View", new ViewAddressModel
        {
            AccessToken = accessToken
        });
        var result = await _http.Get(url, true);
        var jResult = JsonConvert.DeserializeObject<ViewLogViewModel>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }

    public async Task<AiurProtocol> DeleteAppAsync(string accessToken, string appId)
    {
        var url = new AiurUrl(_observerLocator.Instance, "Event", "DeleteApp", new { });
        var form = new AiurUrl(string.Empty, new DeleteAppAddressModel
        {
            AccessToken = accessToken,
            AppId = appId
        });
        var result = await _http.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
}