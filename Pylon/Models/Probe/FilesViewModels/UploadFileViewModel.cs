using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe.FilesViewModels
{
    public class UploadFileViewModel : AiurProtocol
    {
        public string SiteName { get; set; }
        public string FilePath { get; set; }
        public string InternetPath { get; set; }
    }
}
