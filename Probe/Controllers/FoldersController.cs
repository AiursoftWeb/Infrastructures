using Aiursoft.Probe.Data;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class FoldersController : Controller
    {
        private readonly ProbeDbContext _dbContext;
        public FoldersController(ProbeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("Folders/ViewContent/{siteName}/{**folderNames}")]
        public async Task<IActionResult> ViewContent(string siteName, string folderNames)
        {
            string[] folders = folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .Include(t => t.Root.SubFolders)
                .Include(t => t.Root.Files)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == siteName.ToLower());
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, "Not found target site!");
            }
            var currentFolder = site.Root;
            foreach (var folder in folders)
            {
                var folderObject = await _dbContext
                    .Folders
                    .Include(t => t.SubFolders)
                    .Include(t => t.Files)
                    .Where(t => t.ContextId == currentFolder.Id)
                    .SingleOrDefaultAsync(t => t.FolderName == folder);
                if (folderObject == null)
                {
                    return this.Protocol(ErrorType.NotFound, $"Not found folder {folder} under folder {currentFolder.FolderName}!");
                }
                currentFolder = folderObject;
            }
            return Json(currentFolder);
        }
    }
}
