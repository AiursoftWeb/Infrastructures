using Microsoft.Extensions.Configuration;

namespace Aiursoft.Pylon.Services
{
    public class ServiceLocation
    {
        public readonly string Account;
        public readonly string API;
        public readonly string Archon;
        public readonly string UI;
        public readonly string Colossus;
        public readonly string Developer;
        public readonly string EE;
        public readonly string Stargate;
        public readonly string StargateListenAddress;
        public readonly string Wiki;
        public readonly string WWW;
        public readonly string Probe;

        public ServiceLocation(IConfiguration configuration)
        {
            var section = configuration.GetSection("Dependencies");
            Account = TrySet(section["AccountPath"], "https://account.aiursoft.com");
            API = TrySet(section["APIPath"], "https://api.aiursoft.com");
            Archon = TrySet(section["ArchonPath"], "https://archon.aiursoft.com");
            UI = TrySet(section["UIPath"], "https://ui.cdn.aiursoft.com");
            Colossus = TrySet(section["ColossusPath"], "https://colossus.aiursoft.com");
            Developer = TrySet(section["DeveloperPath"], "https://developer.aiursoft.com");
            EE = TrySet(section["EEPath"], "https://ee.aiursoft.com");
            Stargate = TrySet(section["StargatePath"], "https://stargate.aiursoft.com");
            Wiki = TrySet(section["WikiPath"], "https://wiki.aiursoft.com");
            WWW = TrySet(section["WWW"], "https://www.aiursoft.com");
            Probe = TrySet(section["Probe"], "https://probe.aiursoft.com");

            StargateListenAddress = Stargate
                .Replace("https://", "wss://")
                .Replace("http://", "ws://");
        }

        private string TrySet(string setting, string defaultValue)
        {
            return string.IsNullOrEmpty(setting) ? defaultValue : setting;
        }
    }
}
