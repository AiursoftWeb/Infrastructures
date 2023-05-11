using System.Text;
using System.Text.RegularExpressions;

namespace Aiursoft.DocGenerator.Tools;

public static class StringExtends
{
    public static string TrimController(this string controllerName)
    {
        return controllerName
            .Replace("Controller", "")
            .Replace("Api", "API");
    }

    public static string SplitStringUpperCase(this string source)
    {
        var split = Regex.Split(source, @"(?<!^)(?=[A-Z])");
        var b = new StringBuilder();
        var first = true;
        foreach (var word in split)
        {
            if (first)
            {
                b.Append(word + " ");
                first = false;
            }
            else
            {
                b.Append(word.ToLower() + " ");
            }
        }

        return b.ToString();
    }
}