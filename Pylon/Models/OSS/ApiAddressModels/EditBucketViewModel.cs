using Aiursoft.Pylon.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class EditBucketAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int BucketId { get; set; }
        [Required]
        [NoSpace]
        [NoDot]
        public string NewBucketName { get; set; }
        public bool OpenToRead { get; set; }
        public bool OpenToUpload { get; set; }
    }
}
