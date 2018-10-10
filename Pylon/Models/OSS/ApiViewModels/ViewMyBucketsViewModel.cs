using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.OSS.ApiViewModels
{
    public class ViewMyBucketsViewModel : AiurProtocal
    {
        public string AppId { get; set; }
        public IEnumerable<Bucket> Buckets { get; set; }
    }
}
