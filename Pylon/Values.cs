using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Pylon
{
    public static class Values
    {
        public static string ProjectName = "Aiursoft";
        public static int DefaultImageId = 739;
        public static string CorpPhoneNumber = "(+86) 8368-5000";
        public static string Schema = "https";
        public static string WsSchema = "wss";
        public static string GitHubOrganizationAddress { get; } = "https://gitzab.com/AiursoftWeb/";
        public static string FacebookAddress { get; } = "https://facebook.com/";
        public static string TwitterAddress { get; } = "https://twitter.com/";
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
            {"Develop", "https://developer.aiursoft.com"},
            {"Company", "#"},
            {"Privacy", "https://www.aiursoft.com/docs/terms"},
            {"Terms", "https://www.aiursoft.com/docs/terms"},
            {"Wiki", "https://wiki.aiursoft.com"},
            {"GitHub", "https://github.com/AiursoftWeb"}
        };
    }
}
