using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.Probe.DownloadAddressModels;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [Route("Download")]
    [LimitPerMin]
    public class DownloadController : Controller
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly FolderLocator _folderLocator;
        private readonly ProbeDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public DownloadController(
            FolderLocator folderLocator,
            ProbeDbContext dbContext,
            IConfiguration configuration)
        {
            _folderLocator = folderLocator;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [Route("InSites/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> InSites(InSitesAddressModel model)
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
            var foldersWithFileName = _folderLocator.SplitStrings(model.FolderNames);
            var fileName = foldersWithFileName.Last();
            var folders = foldersWithFileName.Take(foldersWithFileName.Count() - 1).ToArray();

            var folder = await _folderLocator.LocateAsync(folders, site.Root);
            var file = folder.Files.SingleOrDefault(t => t.FileName == fileName);
            if (file == null)
            {
                return NotFound();
            }
            var path = _configuration["StoragePath"] + $"{_}Storage{_}{file.Id}.dat";
            return await this.AiurFile(path, file.FileName);
        }
    }
}
