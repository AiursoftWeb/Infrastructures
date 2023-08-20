using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Identity;

public static class AuthValues
{
    public static string DefaultImagePath = "usericon/default.png";
    public static KeyValuePair<string, string> DirectShowString => new("show", "direct");

    public static PasswordOptions PasswordOptions => new()
    {
        RequireDigit = false,
        RequiredLength = 6,
        RequireLowercase = false,
        RequireUppercase = false,
        RequireNonAlphanumeric = false
    };
}