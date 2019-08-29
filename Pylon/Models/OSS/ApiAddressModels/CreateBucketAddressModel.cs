using Aiursoft.Pylon.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class CreateBucketAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(5)]
        [NoSpace]
        public string BucketName { get; set; }
        public bool OpenToRead { get; set; }
        public bool OpenToUpload { get; set; }
    }
}
