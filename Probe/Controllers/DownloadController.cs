using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.DownloadAddressModels;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
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
        private readonly AiurCache _cache;

        public DownloadController(
            FolderLocator folderLocator,
            ProbeDbContext dbContext,
            IConfiguration configuration,
            ImageCompressor imageCompressor,
            TokenEnsurer tokenEnsurer,
            AiurCache cache)
        {
            _folderLocator = folderLocator;
            _dbContext = dbContext;
            _configuration = configuration;
            _imageCompressor = imageCompressor;
            _tokenEnsurer = tokenEnsurer;
            _cache = cache;
        }

        [Route(template: "File/{SiteName}/{**FolderNames}", Name = "File")]
        [Route(template: "Open/{SiteName}/{**FolderNames}", Name = "Open")]
        public async Task<IActionResult> Open(OpenAddressModel model)
        {
            var site = await GetSiteWithCache(model.SiteName);
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
                var file = await GetFileWithCache(folders, fileName, site.Root);
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

        private Task<Site> GetSiteWithCache(string siteName)
        {
            return _cache.GetAndCache($"site.info.{siteName}", () =>
               _dbContext
               .Sites
               .Include(t => t.Root)
               .Include(t => t.Root.SubFolders)
               .Include(t => t.Root.Files)
               .SingleOrDefaultAsync(t => t.SiteName.ToLower() == siteName));
        }

        public Task<SDK.Models.File> GetFileWithCache(string[] folders, string fileName, Folder root)
        {
            return _cache.GetAndCache($"file.info.{string.Join(".", folders)}.{fileName}", async () =>
            {
                var folder = await _folderLocator.LocateAsync(folders, root, false);
                var file = folder.Files.SingleOrDefault(t => t.FileName == fileName);
                return file;
            });
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
