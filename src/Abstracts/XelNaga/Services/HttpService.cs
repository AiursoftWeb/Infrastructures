using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.XelNaga.Services;

/// <summary>
///     An HTTP client which only returns success data.
/// </summary>
public class HttpService : IScopedDependency
{
    private readonly HttpClient _client;

    public HttpService(
        IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
    }

    public async Task<string> Get(AiurUrl url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url.ToString())
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>())
        };

        using var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await GetResponseContent(response);
        }

        throw new WebException(
            $"The remote server returned unexpected status code: {response.StatusCode} - {response.ReasonPhrase}. Url: {url}");
    }

    public async Task<string> Post(AiurUrl url, AiurUrl postDataStr)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url.ToString())
        {
            Content = new FormUrlEncodedContent(postDataStr.Params)
        };

        using var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await GetResponseContent(response);
        }

        throw new WebException(
            $"The remote server returned unexpected status code: {response.StatusCode} - {response.ReasonPhrase}. Url: {url}");
    }

    public async Task<string> PostWithFile(AiurUrl url, Stream fileStream)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url.Address)
        {
            Content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "file", "file" }
            }
        };

        using var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await GetResponseContent(response);
        }

        throw new WebException(
            $"The remote server returned unexpected status code: {response.StatusCode} - {response.ReasonPhrase}. Url: {url}");
    }

    private async Task<string> GetResponseContent(HttpResponseMessage response)
    {
        var isGZipEncoded = response.Content.Headers.ContentEncoding.Contains("gzip");
        if (isGZipEncoded)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var decompressionStream = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(decompressionStream);
            var text = await reader.ReadToEndAsync();
            return text;
        }
        else
        {
            var text = await response.Content.ReadAsStringAsync();
            return text;
        }
    }
}