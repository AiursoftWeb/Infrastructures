using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.Probe.FilesAddressModels;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [Route("Files")]
    public class FilesController
    {
        private readonly ProbeDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;
        private readonly FolderLocator _folderLocator;

        public FilesController(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager,
            FolderLocator folderLocator)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
            _folderLocator = folderLocator;
        }

        [HttpPost]
        [Route("UploadFile/{SiteName}/{**FolderNames}")]
        [FileChecker]
        public async Task<IActionResult> UploadFile(UploadFileAddressModel model)
        {
            await Task.Delay(0);
            throw new NotImplementedException();
        }
    }
}
