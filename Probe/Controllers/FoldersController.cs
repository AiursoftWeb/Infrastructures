using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
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
        private readonly FolderLocator _folderLocator;

        public FoldersController(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager,
            FolderLocator folderLocator)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
            _folderLocator = folderLocator;
        }

        [Route("ViewContent/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> ViewContent(ViewContentAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .Include(t => t.Root.SubFolders)
                .Include(t => t.Root.Files)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == model.SiteName.ToLower());
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, "Not found target site!");
            }
            if (site.AppId != appid)
            {
                return this.Protocol(ErrorType.Unauthorized, "The target folder is not your app's folder!");
            }
            var folder = await _folderLocator.LocateAsync(model.FolderNames, site.Root);
            return Json(folder);
        }

        [HttpPost]
        [Route("CreateNewFolder/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> CreateNewFolder(CreateNewFolderAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .Include(t => t.Root.SubFolders)
                .Include(t => t.Root.Files)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == model.SiteName.ToLower());
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, "Not found target site!");
            }
            if (site.AppId != appid)
            {
                return this.Protocol(ErrorType.Unauthorized, "The target site is not your app's site!");
            }
            var folder = await _folderLocator.LocateAsync(model.FolderNames, site.Root);
            var conflict = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .AnyAsync(t => t.FolderName == model.NewFolderName.ToLower());
            if (conflict)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"Folder name: '{model.NewFolderName}' conflict!");
            }
            var newFolder = new Folder
            {
                ContextId = folder.Id,
                FolderName = model.NewFolderName.ToLower(),
            };
            _dbContext.Folders.Add(newFolder);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully created your new folder!");
        }
    }
}
