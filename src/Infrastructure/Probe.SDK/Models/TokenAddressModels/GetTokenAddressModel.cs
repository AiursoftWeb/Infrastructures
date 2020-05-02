using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.TokenAddressModels
{
    public class GetTokenAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        public string UnderPath { get; set; }
        /// <summary>
        /// Upload, Download
        /// </summary>
        [Required]
        public string Permissions { get; set; }
    }
}
