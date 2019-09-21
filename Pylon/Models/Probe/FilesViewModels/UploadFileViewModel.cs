namespace Aiursoft.Pylon.Models.Probe.FilesViewModels
{
    public class UploadFileViewModel : AiurProtocol
    {
        /// <summary>
        /// For example: mynewsite
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// For example: pathA/pathB/file.extension. Caution: This property is not UrlEncoded!
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// For example: https://probe.aiursoft.com/Download/Open/mynewsite/img_20190727_143308.jpg. Caution: This property is encoded and you can access it directly.
        /// </summary>
        public string InternetPath { get; set; }
        public long FileSize { get; set; }
    }
}
