using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class ServiceLocation
    {
        public readonly string Account;
        public readonly string API;
        public readonly string CDN;
        public readonly string Colossus;
        public readonly string Developer;
        public readonly string EE;
        public readonly string OSS;
        public readonly string Stargate;
        public readonly string StargateListenAddress;
        public readonly string Wiki;
        public readonly string WWW;

        public ServiceLocation(IConfiguration configuration)
        {
            Account = configuration["Dependencies:AccountPath"];
            API = configuration["Dependencies:APIPath"];
            CDN = configuration["Dependencies:CDNPath"];
            Colossus = configuration["Dependencies:ColossusPath"];
            Developer = configuration["Dependencies:DeveloperPath"];
            EE = configuration["Dependencies:EEPath"];
            OSS = configuration["Dependencies:OSSPath"];
            Stargate = configuration["Dependencies:StargatePath"];
            StargateListenAddress = Stargate
                .Replace("https://", "wss://")
                .Replace("http://", "ws://");
            Wiki = configuration["Dependencies:WikiPath"];
            WWW = configuration["Dependencies:WWW"];
        }
    }
}
