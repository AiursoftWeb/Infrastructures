using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Services;

[Obsolete("Use HTTP service for external and ApiProxyService for internal.")]
public static class SimpleHttp
{
    [Obsolete("Use HTTP service for external and ApiProxyService for internal.")]
    public static async Task<string> DownloadAsString(string url)
    {
        var http = new HttpClient();
        var response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return body;
    }
}