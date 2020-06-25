namespace Aiursoft.XelNaga.Services
{
    public static class ScriptsFilter
    {
        public static string FilterString(string html)
        {
            return html.Replace("<scripts", "< scripts");
        }
    }
}
