using System;

namespace Aiursoft.Pylon.Models.OSS.ApiViewModels
{
    [Obsolete]
    public class UploadFileViewModel : AiurProtocol
    {
        public int FileKey { get; set; }
        public string Path { get; set; }
    }
}
