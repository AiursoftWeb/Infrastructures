namespace Aiursoft.XelNaga.Services;

// TODO: Move to WebTools.
public static class ScriptsFilter
{
    public static string FilterString(string html)
    {
        return html.Replace("<scripts", "< scripts");
    }
}