using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.FilesAddressModels;
using Aiursoft.Pylon.Models.Probe.FilesViewModels;
using Aiursoft.Pylon.Models.Probe.FoldersAddressModels;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [Route("Files")]
    public class FilesController : Controller
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly ProbeDbContext _dbContext;
        private readonly FolderLocator _folderLocator;
        private readonly IConfiguration _configuration;
        private readonly FolderCleaner _folderCleaner;
        private readonly FolderRefactor _folderRefactor;
        private readonly ServiceLocation _serviceLocation;
        private readonly static object _obj = new object();

        public FilesController(
            ProbeDbContext dbContext,
            FolderLocator folderLocator,
            FolderCleaner folderCleaner,
            IConfiguration configuration,
            FolderRefactor folderRefactor,
            ServiceLocation serviceLocation)
        {
            _dbContext = dbContext;
            _folderLocator = folderLocator;
            _configuration = configuration;
            _folderRefactor = folderRefactor;
            _serviceLocation = serviceLocation;
            _folderCleaner = folderCleaner;
        }

        [HttpPost]
        [Route("UploadFile/{SiteName}/{**FolderNames}")]
        [FileChecker]
        [APIModelStateChecker]
        public async Task<IActionResult> UploadFile(UploadFileAddressModel model)
        {
            var folders = _folderLocator.SplitStrings(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders);
            var file = Request.Form.Files.First();
            var newFile = new Pylon.Models.Probe.File
            {
                FileName = Path.GetFileName(file.FileName).ToLower(),
                ContextId = folder.Id
            };
            //Ensure there not exists file with the same file name.
            lock (_obj)
            {
                var exists = folder.Files.Any(t => t.FileName == newFile.FileName);
                if (exists)
                {
                    return this.Protocol(ErrorType.HasDoneAlready, "There already exists a file with that name.");
                }
                //Save to database
                _dbContext.Files.Add(newFile);
                _dbContext.SaveChanges();
            }
            //Try saving file.
            var directoryPath = _configuration["StoragePath"] + $"{_}Storage{_}";
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (var fileStream = new FileStream(directoryPath + newFile.Id + ".dat", FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }
            var filePath = $"{model.FolderNames}/{newFile.FileName}".TrimStart('/');
            var path = $"{_serviceLocation.Probe}/Download/{nameof(DownloadController.InSites)}/{model.SiteName}/{filePath}";
            return Json(new UploadFileViewModel
            {
                InternetPath = path,
                SiteName = model.SiteName,
                FilePath = model.FolderNames,
                Code = ErrorType.Success,
                Message = "Successfully uploaded your file."
            });
        }

        [HttpPost]
        [APIModelStateChecker]
        [Route("DeleteFile/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> DeleteFile(DeleteFileAddressModel model)
        {
            var file = await _folderLocator.LocateSiteAndFile(model.AccessToken, model.SiteName, _folderLocator.SplitStrings(model.FolderNames));
            if (file == null)
            {
                return this.Protocol(ErrorType.NotFound, "The file cannot be found. Maybe it has been deleted.");
            }
            _folderCleaner.DeleteFile(file);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"Successfully deleted the file '{file.FileName}'");
        }

        [HttpPost]
        [APIModelStateChecker]
        [Route("DeleteFile/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> MoveFile(MoveFolderAddressModel model)
        {
            await _folderRefactor.MoveFile(model.AccessToken, model.SiteName, _folderLocator.SplitStrings(model.FolderNames),
                _folderLocator.SplitStrings(model.NewFolderNames));
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully moved your file.");
        }
    }
}
