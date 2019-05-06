using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Models.Probe.FilesAddressModels;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly static object _obj = new object();

        public FilesController(
            ProbeDbContext dbContext,
            FolderLocator folderLocator,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _folderLocator = folderLocator;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("UploadFile/{SiteName}/{**FolderNames}")]
        [FileChecker]
        [APIModelStateChecker]
        public async Task<IActionResult> UploadFile(UploadFileAddressModel model)
        {
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, model.FolderNames);
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
            string directoryPath = _configuration["StoragePath"] + $"{_}Storage{_}";
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (var fileStream = new FileStream(directoryPath + newFile.Id + ".dat", FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }
            return this.Protocol(ErrorType.Success, "Successfully uploaded your file.");
        }
    }
}
