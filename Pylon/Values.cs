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
        public static int DefaultImageId = 10;
        public static string LoadingImage = "https://cdn.aiursoft.com/images/loading.gif";
        public static string CorpPhoneNumber = "(+86) 8368-5000";
        public static string Schema = "https";
        public static string WSSchema = "wss";
        public static long MaxFileSize = 1000 * 1024 * 1024;
        public static string GitHubOrganizationAddress { get; private set; } = "https://gitzab.com/AiursoftWeb/";
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
            {"Privacy", "https://www.aiursoft.com/docs/terms"},
            {"Terms", "https://www.aiursoft.com/docs/terms"},
            {"Wiki", "https://wiki.aiursoft.com"},
            {"GitHub", "https://github.com/AiursoftWeb"}
        };
    }
}
