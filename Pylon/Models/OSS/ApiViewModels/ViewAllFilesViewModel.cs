using System.Collections.Generic;

namespace Aiursoft.Pylon.Models.OSS.ApiViewModels
{
    public class ViewAllFilesViewModel : AiurProtocol
    {
        public virtual int BucketId { get; set; }
        public IEnumerable<OSSFile> AllFiles { get; set; }
    }
}
