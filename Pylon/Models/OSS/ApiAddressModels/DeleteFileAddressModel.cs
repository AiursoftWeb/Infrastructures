using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class DeleteFileAddressModel
    {
        public string AccessToken { get; set; }
        public int FileKey { get; set; }
        public int BucketId { get; set; }
    }
}