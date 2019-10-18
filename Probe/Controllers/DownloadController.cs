using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.DownloadAddressModels;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [LimitPerMin]
    [Route("Download")]
    [APIExpHandler]
    public class DownloadController : Controller
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly FolderLocator _folderLocator;
        private readonly ProbeDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ImageCompressor _imageCompressor;
        private readonly TokenEnsurer _tokenEnsurer;

        public DownloadController(
            FolderLocator folderLocator,
            ProbeDbContext dbContext,
            IConfiguration configuration,
            ImageCompressor imageCompressor,
            TokenEnsurer tokenEnsurer)
        {
            _folderLocator = folderLocator;
            _dbContext = dbContext;
            _configuration = configuration;
            _imageCompressor = imageCompressor;
            _tokenEnsurer = tokenEnsurer;
        }

        [Route(template: "File/{SiteName}/{**FolderNames}", Name = "File")]
        [Route(template: "Open/{SiteName}/{**FolderNames}", Name = "Open")]
        public async Task<IActionResult> Open(OpenAddressModel model)
        {
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .Include(t => t.Root.SubFolders)
                .Include(t => t.Root.Files)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == model.SiteName);
            if (site == null)
            {
                return NotFound();
            }
            if (!site.OpenToDownload)
            {
                _tokenEnsurer.Ensure(model.PBToken, "Download", model.SiteName, model.FolderNames);
            }
            var (folders, fileName) = _folderLocator.SplitToPath(model.FolderNames);
            try
            {
                var folder = await _folderLocator.LocateAsync(folders, site.Root, false);
                var file = folder.Files.SingleOrDefault(t => t.FileName == fileName);
                if (file == null)
                {
                    return NotFound();
                }
                var path = _configuration["StoragePath"] + $"{_}Storage{_}{file.Id}.dat";
                var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLower();
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
            int.TryParse(Request.Query["w"], out int w);
            int.TryParse(Request.Query["h"], out int h);
            if (h >= 0 && w >= 0 && h + w > 0)
            {
                return this.WebFile(await _imageCompressor.Compress(path, w, h), extension);
            }
            else
            {
                return this.WebFile(await _imageCompressor.ClearExif(path), extension);
            }
        }
    }
}
