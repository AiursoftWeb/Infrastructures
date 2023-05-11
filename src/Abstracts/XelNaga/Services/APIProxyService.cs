using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.Logging;

namespace Aiursoft.XelNaga.Services;

public class APIProxyService : IScopedDependency
{
    private readonly HttpClient _client;
    private readonly Regex _regex;
    private readonly ILogger<APIProxyService> _logger;

    public APIProxyService(
        IHttpClientFactory clientFactory,
        ILogger<APIProxyService> logger)
    {
        _regex = new Regex("^https://", RegexOptions.Compiled);
        _client = clientFactory.CreateClient();
        _logger = logger;
    }

    private Task<HttpResponseMessage> SendWithRetry(HttpRequestMessage request)
    {
        return AsyncHelper.Try(async () =>
        {
            var response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.BadGateway ||
                response.StatusCode == HttpStatusCode.ServiceUnavailable)
                throw new WebException(
                    $"Api proxy failed bacause bad gateway [{response.StatusCode}]. (This error will trigger auto retry)");
            return response;
        }, 5, e => { _logger.LogCritical(e, e.Message); });
    }

    public async Task<string> Get(AiurUrl url, bool forceHttp = false, bool autoRetry = true)
    {
        if (forceHttp && !url.IsLocalhost()) url.Address = _regex.Replace(url.Address, "http://");

        var request = new HttpRequestMessage(HttpMethod.Get, url.ToString())
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>())
        };

        request.Headers.Add("X-Forwarded-Proto", "https");
        request.Headers.Add("accept", "application/json, text/html");

        using var response = autoRetry ? await SendWithRetry(request) : await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (content.IsValidJson()) return content;

        if (response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"The {nameof(APIProxyService)} can only handle JSON content while the remote server returned unexpected content: {content.OTake(100)}.");
        throw new WebException(
            $"The remote server returned unexpected content: {content.OTake(100)}. code: {response.StatusCode} - {response.ReasonPhrase}.");
    }

    public async Task<string> Post(AiurUrl url, AiurUrl postDataStr, bool forceHttp = false, bool autoRetry = true)
    {
        if (forceHttp && !url.IsLocalhost()) url.Address = _regex.Replace(url.Address, "http://");

        var request = new HttpRequestMessage(HttpMethod.Post, url.ToString())
        {
            Content = new FormUrlEncodedContent(postDataStr.Params)
        };

        request.Headers.Add("X-Forwarded-Proto", "https");
        request.Headers.Add("accept", "application/json");

        using var response = autoRetry ? await SendWithRetry(request) : await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (content.IsValidJson()) return content;

        if (response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"The {nameof(APIProxyService)} can only handle JSON content while the remote server returned unexpected content: {content.OTake(100)}.");
        throw new WebException(
            $"The remote server returned unexpected content: {content.OTake(100)}. code: {response.StatusCode} - {response.ReasonPhrase}.");
    }

    public async Task<string> PostWithFile(AiurUrl url, Stream fileStream, bool forceHttp = false,
        bool autoRetry = true)
    {
        if (forceHttp && !url.IsLocalhost()) url.Address = _regex.Replace(url.Address, "http://");
        var request = new HttpRequestMessage(HttpMethod.Post, url.Address)
        {
            Content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "file", "file" }
            }
        };

        request.Headers.Add("X-Forwarded-Proto", "https");
        request.Headers.Add("accept", "application/json");

        using var response = autoRetry ? await SendWithRetry(request) : await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (content.IsValidJson()) return content;

        if (response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"The {nameof(APIProxyService)} can only handle JSON content while the remote server returned unexpected content: {content.OTake(100)}.");
        throw new WebException(
            $"The remote server returned unexpected content: {content.OTake(100)}. code: {response.StatusCode} - {response.ReasonPhrase}.");
    }
}