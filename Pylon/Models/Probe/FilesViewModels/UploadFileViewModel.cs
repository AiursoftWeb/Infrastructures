using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe.FilesViewModels
{
    public class UploadFileViewModel : AiurProtocol
    {
        /// <summary>
        /// For example: mynewsite
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// For example: pathA/pathB/file.extension.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// For example: https://probe.aiursoft.com/Download/InSites/mynewsite/img_20190727_143308.jpg
        /// </summary>
        public string InternetPath { get; set; }
    }
}
