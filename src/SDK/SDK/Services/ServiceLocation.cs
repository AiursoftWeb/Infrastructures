using System.Net.Http;
using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.SDK.Services;

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
        Wiki = TrySet(section["WikiPath"], "https://wiki.aiursoft.com");
        WWW = TrySet(section["WWWPath"], "https://www.aiursoft.com");
        Status = TrySet(section["StatusPath"], "https://status.aiursoft.com");
    }

    public string Account { get; private set; }
    public string UI { get; private set; }
    public string Wiki { get; private set; }
    public string WWW { get; private set; }
    public string Status { get; private set; }

    private string TrySet(string setting, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(setting) ? defaultValue : setting;
    }
}