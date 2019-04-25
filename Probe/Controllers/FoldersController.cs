using Aiursoft.Probe.Data;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.FoldersAddressModels;
using Aiursoft.Pylon.Services;
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
    [Route("Folders")]
    public class FoldersController : Controller
    {
        private readonly ProbeDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;

        public FoldersController(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
        }

        [Route("ViewContent/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> ViewContent(ViewContentAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            string[] folders = model.FolderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            var site = await _dbContext
                .Sites
                .Where(t => t.AppId == appid)
                .Include(t => t.Root)
                .Include(t => t.Root.SubFolders)
                .Include(t => t.Root.Files)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == model.SiteName.ToLower());
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
