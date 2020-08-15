using Aiursoft.Gateway.SDK.Models;

namespace Aiursoft.Colossus.Models
{
    public enum SiteType
    {
        WebSite,
        FileBrowser
    }

    public class ColossusUser : AiurUserBase
    {
        public string SiteName { get; set; }
        public SiteType SiteType { get; set; }
    }
}
