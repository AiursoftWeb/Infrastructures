using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Repositories;
using Aiursoft.Probe.SDK.Models.FilesAddressModels;
using Aiursoft.Probe.SDK.Models.FilesViewModels;
using Aiursoft.Probe.SDK.Services;
using Aiursoft.Probe.Services;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [LimitPerMin]
    [APIExpHandler]
    [APIModelStateChecker]
    [Route("Files")]
    [DisableRequestSizeLimit]
    public class FilesController : ControllerBase
    {
        private readonly FolderSplitter _folderSplitter;
        private readonly TokenEnsurer _tokenEnsurer;
        private readonly ProbeLocator _probeLocator;
        private readonly IStorageProvider _storageProvider;
        private readonly SiteRepo _siteRepo;
        private readonly FolderRepo _folderRepo;
        private readonly FileRepo _fileRepo;

        public FilesController(
            FolderSplitter folderLocator,
            TokenEnsurer tokenEnsurer,
            ProbeLocator probeLocator,
            IStorageProvider storageProvider,
            SiteRepo siteRepo,
            FolderRepo folderRepo,
            FileRepo fileRepo)
        {
            _folderSplitter = folderLocator;
            _tokenEnsurer = tokenEnsurer;
            _probeLocator = probeLocator;
            _storageProvider = storageProvider;
            _siteRepo = siteRepo;
            _folderRepo = folderRepo;
            _fileRepo = fileRepo;
        }

        [HttpPost]
        [Route("UploadFile/{SiteName}/{**FolderNames}")]
        [APIProduces(typeof(UploadFileViewModel))]
        public async Task<IActionResult> UploadFile(UploadFileAddressModel model)
        {
            var site = await _siteRepo.GetSiteByName(model.SiteName);
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can't find a site with name: '{model.SiteName}'!");
            }
            if (!site.OpenToUpload)
            {
                _tokenEnsurer.Ensure(model.Token, "Upload", model.SiteName, model.FolderNames);
            }
            var folders = _folderSplitter.SplitToFolders(model.FolderNames);
            var rootFolder = await _folderRepo.GetFolderFromId(site.RootFolderId);
            var folder = await _folderRepo.GetFolderFromPath(folders, rootFolder, model.RecursiveCreate);
            if (folder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Can't find your folder!");
            }
            // Executing here will let the browser upload the file.
            try
            {
                var _ = HttpContext.Request.Form.Files.FirstOrDefault()?.ContentType;
            }
            catch (InvalidOperationException e)
            {
                return this.Protocol(ErrorType.InvalidInput, e.Message);
            }
            if (HttpContext.Request.Form.Files.Count < 1)
            {
                return this.Protocol(ErrorType.InvalidInput, "Please provide a file!");
            }
            var file = HttpContext.Request.Form.Files.First();
            if (!new ValidFolderName().IsValid(file.FileName))
            {
                return this.Protocol(ErrorType.InvalidInput, $"Invalid file name: '{file.FileName}'!");
            }
            var fileName = _folderSplitter.GetValidFileName(folder.Files.Select(t => t.FileName), file.FileName);
            var newFileHardwareId = await _fileRepo.SaveFileToDb(fileName, folder.Id, file.Length);
            await _storageProvider.Save(newFileHardwareId, file);
            var filePath = _probeLocator.GetProbeFullPath(model.SiteName, string.Join('/', folders), fileName);
            return this.Protocol(new UploadFileViewModel
            {
                InternetPath = _probeLocator.GetProbeOpenAddress(filePath),
                DownloadPath = _probeLocator.GetProbeDownloadAddress(filePath),
                SiteName = model.SiteName,
                FilePath = filePath,
                FileSize = file.Length,
                Code = ErrorType.Success,
                Message = "Successfully uploaded your file."
            });
        }

        [HttpPost]
        [Route("DeleteFile/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> DeleteFile(DeleteFileAddressModel model)
        {
            var (folders, fileName) = _folderSplitter.SplitToFoldersAndFile(model.FolderNames);
            var folder = await _folderRepo.GetFolderAsOwner(model.AccessToken, model.SiteName, folders);
            if (folder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate folder failed!");
            }
            var file = await _fileRepo.GetFileInFolder(folder, fileName);
            if (file == null)
            {
                return this.Protocol(ErrorType.NotFound, "The file cannot be found. Maybe it has been deleted.");
            }
            await _fileRepo.DeleteFileById(file.Id);
            return this.Protocol(ErrorType.Success, $"Successfully deleted the file '{file.FileName}'");
        }

        [HttpPost]
        [Route("CopyFile/{SiteName}/{**FolderNames}")]
        [APIProduces(typeof(UploadFileViewModel))]
        public async Task<IActionResult> CopyFile(CopyFileAddressModel model)
        {
            var (sourceFolders, sourceFileName) = _folderSplitter.SplitToFoldersAndFile(model.FolderNames);
            var targetFolders = _folderSplitter.SplitToFolders(model.TargetFolderNames);
            var sourceFolder = await _folderRepo.GetFolderAsOwner(model.AccessToken, model.SiteName, sourceFolders);
            var targetFolder = await _folderRepo.GetFolderAsOwner(model.AccessToken, model.TargetSiteName, targetFolders, true);
            if (sourceFolder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate source folder failed!");
            }
            if (targetFolder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate target folder failed!");
            }
            var file = sourceFolder.Files.SingleOrDefault(t => t.FileName == sourceFileName);
            if (file == null)
            {
                return this.Protocol(ErrorType.NotFound, "The file cannot be found. Maybe it has been deleted.");
            }
            var fileName = _folderSplitter.GetValidFileName(targetFolder.Files.Select(t => t.FileName), file.FileName);
            await _fileRepo.CopyFile(fileName, file.FileSize, targetFolder.Id, file.HardwareId);
            var filePath = _probeLocator.GetProbeFullPath(model.TargetSiteName, string.Join('/', targetFolders), fileName);
            var internetPath = _probeLocator.GetProbeOpenAddress(filePath);
            return this.Protocol(new UploadFileViewModel
            {
                InternetPath = internetPath,
                SiteName = model.TargetSiteName,
                FilePath = filePath,
                FileSize = file.FileSize,
                Code = ErrorType.Success,
                Message = "Successfully copied your file."
            });
        }

        [HttpPost]
        [Route("RenameFile/{SiteName}/{**Foldernames}")]
        public async Task<IActionResult> RenameFile(RenameFileAddressModel model)
        {
            var (sourceFolders, sourceFileName) = _folderSplitter.SplitToFoldersAndFile(model.FolderNames);
            var sourceFolder = await _folderRepo.GetFolderAsOwner(model.AccessToken, model.SiteName, sourceFolders);
            var file = sourceFolder.Files.SingleOrDefault(t => t.FileName == sourceFileName);
            if (file == null)
            {
                return this.Protocol(ErrorType.NotFound, "The file cannot be found. Maybe it has been deleted.");
            }
            
            var newFileName = _folderSplitter.GetValidFileName(sourceFolder.Files.Select(t => t.FileName), model.TargetFileName);
            await _fileRepo.UpdateName(file.Id , newFileName);

            var filePath = _probeLocator.GetProbeFullPath(model.SiteName, string.Join('/', sourceFileName), newFileName);
            var internetPath = _probeLocator.GetProbeOpenAddress(filePath);
            return this.Protocol(new UploadFileViewModel
            {
                InternetPath = internetPath,
                SiteName = model.SiteName,
                FilePath = filePath,
                FileSize = file.FileSize,
                Code = ErrorType.Success,
                Message = "Successfully copied your file."
            });
        }
    }
}
