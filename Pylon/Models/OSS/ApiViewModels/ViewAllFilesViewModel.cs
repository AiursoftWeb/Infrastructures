using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.OSS.ApiViewModels
{
    public class ViewAllFilesViewModel : AiurProtocal
    {
        public virtual int BucketId { get; set; }
        public IEnumerable<OSSFile> AllFiles { get; set; }
    }
}
