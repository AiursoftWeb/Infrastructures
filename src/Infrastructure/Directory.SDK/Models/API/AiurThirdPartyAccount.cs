using System;

namespace Aiursoft.Directory.SDK.Models.API;

public class AiurThirdPartyAccount
{
    public string Name { get; set; }
    public string ProviderName { get; set; }
    public DateTime BindTime { get; set; } = DateTime.UtcNow;
}