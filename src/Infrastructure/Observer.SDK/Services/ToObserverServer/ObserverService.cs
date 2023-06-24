using System;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.Observer.SDK.Configuration;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Models.EventAddressModels;
using Aiursoft.Observer.SDK.Models.EventViewModels;
using Aiursoft.Scanner.Abstraction;
using Microsoft.Extensions.Options;

namespace Aiursoft.Observer.SDK.Services.ToObserverServer;

public class ObserverService : IScopedDependency
{
    private readonly AiurProtocolClient _http;
    private readonly ObserverConfiguration _observerLocator;

    public ObserverService(
        AiurProtocolClient http,
        IOptions<ObserverConfiguration> observerLocator)
    {
        _http = http;
        _observerLocator = observerLocator.Value;
    }

    public Task<AiurResponse> LogExceptionAsync(string accessToken, Exception e, string httpRequestPath = "Inline")
    {
        return LogAsync(accessToken, e.Message, e.StackTrace, EventLevel.Exception, httpRequestPath);
    }

    public async Task<AiurResponse> LogAsync(string accessToken, string message, string stackTrace,
        EventLevel eventLevel, string path)
    {
        var url = new AiurApiEndpoint(_observerLocator.Instance, "Event", "Log", new { });
        var form = new AiurApiPayload(new LogAddressModel
        {
            AccessToken = accessToken,
            Message = message,
            StackTrace = stackTrace,
            EventLevel = eventLevel,
            Path = path
        });
        return await _http.Post<AiurResponse>(url, form);
    }

    public async Task<ViewLogViewModel> ViewAsync(string accessToken)
    {
        var url = new AiurApiEndpoint(_observerLocator.Instance, "Event", "View", new ViewAddressModel
        {
            AccessToken = accessToken
        });
        return await _http.Get<ViewLogViewModel>(url);
    }

    public async Task<AiurResponse> DeleteAppAsync(string accessToken, string appId)
    {
        var url = new AiurApiEndpoint(_observerLocator.Instance, "Event", "DeleteApp", new { });
        var form = new AiurApiPayload(new DeleteAppAddressModel
        {
            AccessToken = accessToken,
            AppId = appId
        });
        return await _http.Post<AiurResponse>(url, form);
    }
}