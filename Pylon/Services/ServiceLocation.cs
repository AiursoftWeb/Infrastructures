using Aiursoft.Pylon.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Pylon.Services
{
    public class ServiceLocation : ISingletonDependency
    {
        public readonly string Account;
        public readonly string Gateway;
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
        public readonly string ProbeIO;
        public readonly string Status;

        public ServiceLocation(IConfiguration configuration)
        {
            var section = configuration.GetSection("Dependencies");
            Account = TrySet(section["AccountPath"], "https://account.aiursoft.com");
            Gateway = TrySet(section["GatewayPath"], "https://gateway.aiursoft.com");
            Archon = TrySet(section["ArchonPath"], "https://archon.aiursoft.com");
            UI = TrySet(section["UIPath"], "https://ui.aiursoft.com");
            Colossus = TrySet(section["ColossusPath"], "https://colossus.aiursoft.com");
            Developer = TrySet(section["DeveloperPath"], "https://developer.aiursoft.com");
            EE = TrySet(section["EEPath"], "https://ee.aiursoft.com");
            Stargate = TrySet(section["StargatePath"], "https://stargate.aiursoft.com");
            Wiki = TrySet(section["WikiPath"], "https://wiki.aiursoft.com");
            WWW = TrySet(section["WWWPath"], "https://www.aiursoft.com");
            Probe = TrySet(section["ProbePath"], "https://probe.aiursoft.com");
            ProbeIO = TrySet(section["ProbeIOPath"], "https://{0}.aiursoft.io");
            Status = TrySet(section["StatusPath"], "https://status.aiursoft.com");

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
