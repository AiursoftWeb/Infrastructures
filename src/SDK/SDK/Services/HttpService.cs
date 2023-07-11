using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstractions;

namespace Aiursoft.SDK.Services;

public class HttpService : IScopedDependency
{
    private readonly HttpClient _client;

    public HttpService(
        IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
    }

    public async Task<string> Get(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>())
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        using var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await GetResponseContent(response);
        }

        throw new WebException(
            $"The remote server returned unexpected status code: {response.StatusCode} - {response.ReasonPhrase}. Url: {url}");
    }

    public async Task<string> Post(string url, Dictionary<string, string> postDataStr)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(postDataStr)
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await GetResponseContent(response);
        }

        throw new WebException(
            $"The remote server returned unexpected status code: {response.StatusCode} - {response.ReasonPhrase}. Url: {url}");
    }

    private static async Task<string> GetResponseContent(HttpResponseMessage response)
    {
        var isGZipEncoded = response.Content.Headers.ContentEncoding.Contains("gzip");
        if (isGZipEncoded)
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var decompressionStream = new GZipStream(stream, CompressionMode.Decompress);
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