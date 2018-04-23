using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Pylon
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
        public readonly string Wiki;
        public readonly string KahlaHome;
        public readonly string KahlaServer;

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
            Wiki = configuration["Dependencies:WikiPath"];
            KahlaHome = configuration["Dependencies:KahlaHomePath"];
            KahlaServer = configuration["Dependencies:KahlaServerPath"];
        }
    }
    public static class Values
    {
        public static string ProjectName = "Aiursoft";
        public static string CorpPhoneNumber = "(+86) 8368-5000";
        public static string Schema = "https";
        public static string WSSchema = "wss";
        public static long MaxFileSize = 1000 * 1024 * 1024;
        public static string Domain { get; private set; } = "aiursoft.com";
        public static string Empty { get; private set; } = Schema + "://" + Domain;
        public static string DeveloperServerAddress { get; private set; } = Schema + "://developer." + Domain;
        public static string ApiServerAddress { get; private set; } = Schema + "://api." + Domain;
        public static string AccountServerAddress { get; private set; } = Schema + "://account." + Domain;
        public static string OssServerAddress { get; private set; } = Schema + "://oss." + Domain;
        public static string CdnServerAddress { get; private set; } = Schema + "://cdn." + Domain;
        public static string WikiServerAddress { get; private set; } = Schema + "://wiki." + Domain;
        public static string StargateServerAddress { get; private set; } = Schema + "://stargate." + Domain;
        public static string StargateListenAddress { get; private set; } = WSSchema + "://stargate." + Domain;
        public static string HrServerAddress { get; private set; } = Schema + "://hr." + Domain;
        public static string WWWServerAddress { get; private set; } = Schema + "://www." + Domain;
        public static string ForumServerAddress { get; private set; } = Schema + "://forum." + Domain;
        public static string KahlaServerAddress { get; private set; } = Schema + "://kahla.server." + Domain;
        public static string KahlaAddress { get; private set; } = Schema + "://kahla." + Domain;
        public static string CompanyAddress { get; private set; } = Schema + "://company." + Domain;
        public static string KahlaAppAddress { get; private set; } = Schema + "://kahla.app." + Domain;
        public static int AppsIconBucketId { get; set; } = 1;
        public static int UsersIconBucketId { get; set; } = 2;
        public static string GitHubOrganizationAddress { get; private set; } = "https://github.com/AiursoftWeb/";
        public static string FacebookAddress { get; private set; } = "https://facebook.com/";
        public static string TwitterAddress { get; private set; } = "https://twitter.com/";
        public static KeyValuePair<string, string> DirectShowString => new KeyValuePair<string, string>("show", "direct");
        public static PasswordOptions PasswordOptions => new PasswordOptions
        {
            RequireDigit = false,
            RequiredLength = 6,
            RequireLowercase = false,
            RequireUppercase = false,
            RequireNonAlphanumeric = false
        };
    }
}
