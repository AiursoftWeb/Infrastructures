using Aiursoft.Handler.Models;

namespace Aiursoft.Probe.SDK.Models.FilesViewModels;

public class UploadFileViewModel : AiurProtocol
{
    /// <summary>
    ///     For example: mynewsite
    /// </summary>
    public string SiteName { get; set; }

    /// <summary>
    ///     For example: pathA/pathB/file.extension. Caution: This property is not UrlEncoded!
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    ///     This property is encoded URL and you can access it directly. Browser will try to open it.
    /// </summary>
    public string InternetPath { get; set; }

    /// <summary>
    ///     This property is encoded URL and you can access it directly. Browser will download it.
    /// </summary>
    public string DownloadPath { get; set; }

    public long FileSize { get; set; }
}