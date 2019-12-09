using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.TokenAddressModels
{
    public class GetUploadTokenAddressModel
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
