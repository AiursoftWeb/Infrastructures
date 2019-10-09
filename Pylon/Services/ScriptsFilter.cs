using Aiursoft.Pylon.Interfaces;

namespace Aiursoft.Pylon.Services
{
    public class ScriptsFilter : ITransientDependency
    {
        public string Filt(string html)
        {
            return html.Replace("<scripts", "< scripts");
        }
    }
}
