using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Services;

public static class SimpleHttp
{
    public static async Task<string> DownloadAsString(string url)
    {
        var http = new HttpClient();
        var response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return body;
    }
}