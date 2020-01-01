using Aiursoft.XelNaga.Interfaces;

namespace Aiursoft.XelNaga.Services
{
    public class ScriptsFilter : ITransientDependency
    {
        public string Filt(string html)
        {
            return html.Replace("<scripts", "< scripts");
        }
    }
}
