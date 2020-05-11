using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Repositories;
using Aiursoft.Probe.SDK.Models.DownloadAddressModels;
using Aiursoft.Probe.Services;
using Aiursoft.SDK.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [Route("Download")]
    [APIExpHandler]
    public class DownloadController : Controller
    {
        private readonly FolderLocator _folderLocator;
        private readonly ImageCompressor _imageCompressor;
        private readonly TokenEnsurer _tokenEnsurer;
        private readonly IStorageProvider _storageProvider;
        private readonly FolderRepo _folderRepo;
        private readonly SiteRepo _siteRepo;

        public DownloadController(
            FolderLocator folderLocator,
            ImageCompressor imageCompressor,
            TokenEnsurer tokenEnsurer,
            IStorageProvider storageProvider,
            FolderRepo folderRepo,
            SiteRepo siteRepo)
        {
            _folderLocator = folderLocator;
            _imageCompressor = imageCompressor;
            _tokenEnsurer = tokenEnsurer;
            _storageProvider = storageProvider;
            _folderRepo = folderRepo;
            _siteRepo = siteRepo;
        }

        [Route(template: "File/{SiteName}/{**FolderNames}", Name = "File")]
        [Route(template: "Open/{SiteName}/{**FolderNames}", Name = "Open")]
        public async Task<IActionResult> Open(OpenAddressModel model)
        {
            var site = await _siteRepo.GetSiteByNameWithCache(model.SiteName);
            if (site == null)
            {
                return NotFound();
            }
            if (!site.OpenToDownload)
            {
                _tokenEnsurer.Ensure(model.PBToken, "Download", model.SiteName, model.FolderNames);
            }
            var (folders, fileName) = _folderLocator.SplitToFoldersAndFile(model.FolderNames);
            try
            {
                var siteRoot = await _folderRepo.GetFolderFromId(site.RootFolderId);
                var folder = await _folderRepo.GetFolderFromPath(folders, siteRoot, false);
                if (folder == null)
                {
                    return NotFound();
                }
                var file = folder.Files.SingleOrDefault(t => t.FileName == fileName);
                if (file == null)
                {
                    return NotFound();
                }
                var path = _storageProvider.GetFilePath(file.HardwareId);
                var extension = _storageProvider.GetExtension(file.FileName);
                if (ControllerContext.ActionDescriptor.AttributeRouteInfo.Name == "File")
                {
                    return this.WebFile(path, "do-not-open");
                }
                else if (file.FileName.IsStaticImage() && Image.DetectFormat(path) != null)
                {
                    return await FileWithImageCompressor(path, extension);
                }
                else
                {
                    return this.WebFile(path, extension);
                }
            }
            catch (AiurAPIModelException e) when (e.Code == ErrorType.NotFound)
            {
                return NotFound();
            }
        }

        private async Task<IActionResult> FileWithImageCompressor(string path, string extension)
        {
            int.TryParse(Request.Query["w"], out int width);
            bool.TryParse(Request.Query["square"], out bool square);
            if (width > 0)
            {
                if (square)
                {
                    return this.WebFile(await _imageCompressor.Compress(path, width, width), extension);
                }
                else
                {
                    return this.WebFile(await _imageCompressor.Compress(path, width, 0), extension);
                }
            }
            else
            {
                return this.WebFile(await _imageCompressor.ClearExif(path), extension);
            }
        }
    }
}
