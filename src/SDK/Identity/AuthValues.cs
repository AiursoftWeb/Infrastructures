using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Aiursoft.Identity
{
    public static class AuthValues
    {
        public static string DefaultImagePath = "usericon/default.png";
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
