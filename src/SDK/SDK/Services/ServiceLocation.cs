using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.SDK.Services;

public class ServiceLocation : ISingletonDependency
{
    public ServiceLocation(IConfiguration configuration)
    {
        var section = configuration.GetSection("Dependencies");
        Account = TrySet(section["AccountPath"], "https://account.aiursoft.com");
        Portal = TrySet(section["PortalPath"], "https://portal.aiursoft.com");
        UI = TrySet(section["UIPath"], "https://ui.aiursoft.com");
        Wiki = TrySet(section["WikiPath"], "https://wiki.aiursoft.com");
        WWW = TrySet(section["WWWPath"], "https://www.aiursoft.com");
        Status = TrySet(section["StatusPath"], "https://status.aiursoft.com");
    }

    public string Account { get; private init; }
    public string Portal { get; private init; }
    public string UI { get; private init; }
    public string Wiki { get; private init; }
    public string WWW { get; private init; }
    public string Status { get; private init; }

    private string TrySet(string setting, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(setting) ? defaultValue : setting;
    }
}