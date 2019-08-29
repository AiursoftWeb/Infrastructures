using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class ViewBucketDetailAddressModel
    {
        [Required]
        public int BucketId { get; set; }
    }
}
