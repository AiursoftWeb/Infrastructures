using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.OSS.ApiViewModels
{
    [Obsolete]
    public class UploadFileViewModel : AiurProtocol
    {
        public int FileKey { get; set; }
        public string Path { get; set; }
    }
}
