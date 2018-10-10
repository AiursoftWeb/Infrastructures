using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class ViewBucketDetailAddressModel
    {
        [Required]
        public int BucketId { get; set; }
    }
}
