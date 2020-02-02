using System;

namespace Aiursoft.SDK.Models.API
{
    public class AiurThirdPartyAccount
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public DateTime BindTime { get; set; } = DateTime.UtcNow;
    }
}
