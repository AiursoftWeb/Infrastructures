using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.SDK.Models.DownloadAddressModels
{
    public class OpenAddressModel
    {
        [FromRoute]
        public string FolderNames { get; set; }
        [FromRoute]
        public string SiteName { get; set; }
        [FromQuery(Name = "token")]
        public string PBToken { get; set; }
    }
}
