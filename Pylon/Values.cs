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

    public static class Values
    {
        public static string ProjectName = "Aiursoft";
        public static string CorpPhoneNumber = "(+86) 8368-5000";
        public static string Schema = "https";
        public static string WSSchema = "wss";
        public static long MaxFileSize = 1000 * 1024 * 1024;
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

        public static readonly Dictionary<string, string> Footer = new Dictionary<string, string>
        {
            {"Home", "https://www.aiursoft.com"},
            {"Develope", "https://developer.aiursoft.com"},
            {"Company", "#"},
            {"Privacy", "https://docs.aiursoft.com/Privacy"},
            {"Terms", "https://docs.aiursoft.com/Statement"},
            {"Security", "#"},
            {"Status", "#"},
            {"Wiki", "https://wiki.aiursoft.com"},
            {"Blog", "#"}
        };
    }
}
