using Aiursoft.Pylon.Models;

namespace Aiursoft.Colossus.Models
{
    public enum SiteType
    {
        WebSite,
        FileBrowser
    }

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ColossusUser : AiurUserBase
    {
        public string SiteName { get; set; }
        public SiteType SiteType { get; set; }
    }
}
