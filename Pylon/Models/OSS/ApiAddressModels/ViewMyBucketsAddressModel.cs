using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class ViewMyBucketsAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
