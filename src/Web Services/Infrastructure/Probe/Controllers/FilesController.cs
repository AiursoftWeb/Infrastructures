using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.FilesAddressModels;
using Aiursoft.Probe.SDK.Models.FilesViewModels;
using Aiursoft.Probe.SDK.Services;
using Aiursoft.Probe.Services;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [Route("Files")]
    [DisableRequestSizeLimit]
    public class FilesController : Controller
    {
        private readonly ProbeDbContext _dbContext;
        private readonly FolderLocator _folderLocator;
        private readonly FolderOperator _folderCleaner;
        private readonly TokenEnsurer _tokenEnsurer;
        private readonly ProbeLocator _probeLocator;
        private readonly IStorageProvider _storageProvider;

        public FilesController(
            ProbeDbContext dbContext,
            FolderLocator folderLocator,
            FolderOperator folderCleaner,
            TokenEnsurer tokenEnsurer,
            ProbeLocator probeLocator,
            IStorageProvider storageProvider)
        {
            _dbContext = dbContext;
            _folderLocator = folderLocator;
            _tokenEnsurer = tokenEnsurer;
            _probeLocator = probeLocator;
            _storageProvider = storageProvider;
            _folderCleaner = folderCleaner;
        }

        [HttpPost]
        [Route("UploadFile/{SiteName}/{**FolderNames}")]
        [APIModelStateChecker]
        [APIProduces(typeof(UploadFileViewModel))]
        public async Task<IActionResult> UploadFile(UploadFileAddressModel model)
        {
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == model.SiteName.ToLower());
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can't find a site with name: '{model.SiteName}'!");
            }
            if (!site.OpenToUpload)
            {
                _tokenEnsurer.Ensure(model.PBToken, "Upload", model.SiteName, model.FolderNames);
            }
            var folders = _folderLocator.SplitToFolders(model.FolderNames);
            var folder = await _folderLocator.LocateAsync(folders, site.Root, model.RecursiveCreate);
            if (folder == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can't locate your folder!");
            }
            try
            {
                var _ = HttpContext.Request.Form.Files.FirstOrDefault()?.ContentType;
            }
            catch (InvalidOperationException e)
            {
                return this.Protocol(ErrorType.InvalidInput, e.Message);
            }
            // Executing here will let the browser upload the file.
            if (HttpContext.Request.Form.Files.Count < 1)
            {
                return this.Protocol(ErrorType.InvalidInput, "Please provide a file!");
            }
            var file = HttpContext.Request.Form.Files.First();
            if (!new ValidFolderName().IsValid(file.FileName))
            {
                return this.Protocol(ErrorType.InvalidInput, $"Invalid file name: '{file.FileName}'!");
            }
            var newFile = new File
            {
                FileName = _folderLocator.GetValidFileName(folder.Files.Select(t => t.FileName), file.FileName), //file.FileName,
                ContextId = folder.Id,
                FileSize = file.Length
            };
            // Save to disk
            await _storageProvider.Save(newFile.HardwareId, file);
            // Save to database
            _dbContext.Files.Add(newFile);
            await _dbContext.SaveChangesAsync();
            var filePath = _probeLocator.GetProbeFullPath(model.SiteName, string.Join('/', folders), newFile.FileName);
            var path = _probeLocator.GetProbeOpenAddress(filePath);
            return Json(new UploadFileViewModel
            {
                InternetPath = path,
                SiteName = model.SiteName,
                FilePath = filePath,
                FileSize = file.Length,
                Code = ErrorType.Success,
                Message = "Successfully uploaded your file."
            });
        }

        [HttpPost]
        [APIModelStateChecker]
        [Route("DeleteFile/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> DeleteFile(DeleteFileAddressModel model)
        {
            var (folders, fileName) = _folderLocator.SplitToFoldersAndFile(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders);
            if (folder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate folder failed!");
            }
            var file = folder.Files.SingleOrDefault(t => t.FileName == fileName);
            if (file == null)
            {
                return this.Protocol(ErrorType.NotFound, "The file cannot be found. Maybe it has been deleted.");
            }
            await _folderCleaner.DeleteFile(file);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"Successfully deleted the file '{file.FileName}'");
        }

        [HttpPost]
        [Route("CopyFile/{SiteName}/{**FolderNames}")]
        [APIModelStateChecker]
        [APIProduces(typeof(UploadFileViewModel))]
        public async Task<IActionResult> CopyFile(CopyFileAddressModel model)
        {
            var (sourceFolders, sourceFileName) = _folderLocator.SplitToFoldersAndFile(model.FolderNames);
            var targetFolders = _folderLocator.SplitToFolders(model.TargetFolderNames);
            var sourceFolder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, sourceFolders);
            var targetFolder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.TargetSiteName, targetFolders);
            if (sourceFolder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate source folder failed!");
            }
            var file = sourceFolder.Files.SingleOrDefault(t => t.FileName == sourceFileName);
            if (file == null)
            {
                return this.Protocol(ErrorType.NotFound, "The file cannot be found. Maybe it has been deleted.");
            }
            if (targetFolder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate target folder failed!");
            }
            var newFile = new File
            {
                FileName = _folderLocator.GetValidFileName(targetFolder.Files.Select(t => t.FileName), file.FileName),
                FileSize = file.FileSize,
                ContextId = targetFolder.Id,
                HardwareId = file.HardwareId,
            };
            _dbContext.Files.Add(newFile);
            await _dbContext.SaveChangesAsync();
            var filePath = _probeLocator.GetProbeFullPath(model.TargetSiteName, string.Join('/', targetFolders), newFile.FileName);
            var internetPath = _probeLocator.GetProbeOpenAddress(filePath);
            return Json(new UploadFileViewModel
            {
                InternetPath = internetPath,
                SiteName = model.TargetSiteName,
                FilePath = filePath,
                FileSize = newFile.FileSize,
                Code = ErrorType.Success,
                Message = "Successfully copied your file."
            });
        }
    }
}
