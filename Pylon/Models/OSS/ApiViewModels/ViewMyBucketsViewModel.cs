using System.Collections.Generic;

namespace Aiursoft.Pylon.Models.OSS.ApiViewModels
{
    public class ViewMyBucketsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public IEnumerable<Bucket> Buckets { get; set; }
    }
}
