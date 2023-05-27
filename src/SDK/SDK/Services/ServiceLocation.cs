using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.SDK.Services;

[Obsolete(error: false, message: "Avoid using this!")]
public class ServiceLocation : ISingletonDependency
{
    private readonly HttpClient _client;

    public ServiceLocation(
        IConfiguration configuration,
        IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
        var section = configuration.GetSection("Dependencies");
        Account = TrySet(section["AccountPath"], "https://account.aiursoft.com");
        UI = TrySet(section["UIPath"], "https://ui.aiursoft.com");
        Gateway = TrySet(section["GatewayPath"], "https://gateway.aiursoft.com");
        Developer = TrySet(section["DeveloperPath"], "https://developer.aiursoft.com");
        Wiki = TrySet(section["WikiPath"], "https://wiki.aiursoft.com");
        WWW = TrySet(section["WWWPath"], "https://www.aiursoft.com");
        Status = TrySet(section["StatusPath"], "https://status.aiursoft.com");
        AsyncHelper.RunSync(async () => { UI = await TryGetCDNDomain(UI); });
    }

    public string Account { get; private set; }
    public string UI { get; private set; }
    public string Developer { get; private set; }
    public string Gateway { get; private set; }
    public string Wiki { get; private set; }
    public string WWW { get; private set; }
    public string Status { get; private set; }

    private string TrySet(string setting, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(setting) ? defaultValue : setting;
    }

    public async Task<string> TryGetCDNDomain(string input)
    {
        var domains = input.Split('.').ToList();
        domains.Insert(1, "cdn");

        var combined = string.Join(".", domains);
        try
        {
            var hasDns = await QueryDNS(combined);
            if (hasDns && await TryConnect(combined))
            {
                return combined;
            }

            return input;
        }
        catch
        {
            return input;
        }
    }

    private async Task<bool> TryConnect(string path)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Head, path);
            await _client.SendAsync(request);
            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    private async Task<bool> QueryDNS(string host)
    {
        try
        {
            _ = await Dns.GetHostEntryAsync(host);
            return true;
        }
        catch
        {
            return false;
        }
    }
}