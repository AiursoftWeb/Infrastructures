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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly FolderLocator _folderLocator;
        private readonly FolderCleaner _folderCleaner;
        private readonly FolderRefactor _folderRefactor;

        public FoldersController(
            ProbeDbContext dbContext,
            FolderLocator folderLocator,
            FolderCleaner folderCleaner,
            FolderRefactor folderRefactor)
        {
            _dbContext = dbContext;
            _folderLocator = folderLocator;
            _folderCleaner = folderCleaner;
            _folderRefactor = folderRefactor;
        }

        [Route("ViewContent/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> ViewContent(ViewContentAddressModel model)
        {
            var folders = _folderLocator.SplitStrings(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders);
            return Json(new AiurValue<Folder>(folder)
            {
                Code = ErrorType.Success,
                Message = "Successfully get your folder!"
            });
        }

        [HttpPost]
        [Route("CreateNewFolder/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> CreateNewFolder(CreateNewFolderAddressModel model)
        {
            var folders = _folderLocator.SplitStrings(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders, model.RecursiveCreate);
            var conflict = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .AnyAsync(t => t.FolderName == model.NewFolderName.ToLower());
            if (conflict)
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"Folder name: '{model.NewFolderName}' conflict!");
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

        [HttpPost]
        [Route("DeleteFolder/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> DeleteFolder(DeleteFolderAddressModel model)
        {
            var folders = _folderLocator.SplitStrings(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders);
            if (folder.ContextId == null)
            {
                return this.Protocol(ErrorType.NotEnoughResources, "We can not delete root folder! If you wanna delete your site, please consider delete your site directly!");
            }
            await _folderCleaner.DeleteFolderAsync(folder);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully deleted your folder!");
        }

        [HttpPost]
        [Route("MoveFolder/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> MoveFolder(MoveFolderAddressModel model)
        {
            await _folderRefactor.MoveFolder(model.AccessToken, model.SiteName,
                _folderLocator.SplitStrings(model.FolderNames), _folderLocator.SplitStrings(model.NewFolderNames));
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully moved your folder.");
        }
    }
}
