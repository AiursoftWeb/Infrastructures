using System;

namespace Aiursoft.Pylon.Models.API
{
    public class AiurThirdPartyAccount
    {
        public string ProviderName { get; set; }
        public DateTime BindTime { get; set; } = DateTime.UtcNow;
    }
}
