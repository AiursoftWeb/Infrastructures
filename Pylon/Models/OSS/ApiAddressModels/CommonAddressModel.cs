using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class CommonAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int BucketId { get; set; }
    }
}
