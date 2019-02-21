using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.OSS.ApiViewModels
{
    public class ViewBucketViewModel : AiurProtocol
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewBucketViewModel() { }
        public ViewBucketViewModel(Bucket b)
        {
            this.BucketId = b.BucketId;
            this.BucketName = b.BucketName;
            this.OpenToRead = b.OpenToRead;
            this.OpenToUpload = b.OpenToUpload;
            this.BelongingAppId = b.BelongingAppId;
        }
        public int BucketId { get; set; }
        public string BucketName { get; set; }
        public bool OpenToRead { get; set; } = true;
        public bool OpenToUpload { get; set; } = false;
        public string BelongingAppId { get; set; }
        public int FileCount { get; set; }
        //public IEnumerable<OSSFile> Files { get; set; }
    }
}
