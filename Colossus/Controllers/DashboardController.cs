using Aiursoft.Colossus.Data;
using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.DashboardViewModels;
using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Services.ToOSSServer;
using Microsoft.Extensions.Configuration;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using System.IO;

namespace Aiursoft.Colossus.Controllers
{
    [AiurForceAuth]
    public class DashboardController : Controller
    {
        private readonly UserManager<ColossusUser> _userManager;
        private readonly ColossusDbContext _dbContext;
        private readonly OSSApiService _ossApiService;
        private readonly IConfiguration _configuration;
        private readonly AppsContainer _appsContainer;
        private readonly StorageService _storageService;

        public DashboardController(
            UserManager<ColossusUser> userManager,
            ColossusDbContext dbContext,
            OSSApiService ossApiService,
            IConfiguration configuration,
            AppsContainer appsContainer,
            StorageService storageService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _ossApiService = ossApiService;
            _configuration = configuration;
            _appsContainer = appsContainer;
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user, 0, "Upload new")
            {
                MaxSize = 1000
            };
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [FileChecker(MaxSize = 1000 * 1024 * 1024)]
        [APIModelStateChecker]
        public async Task<IActionResult> Upload()
        {
            var user = await GetCurrentUserAsync();
            var file = Request.Form.Files.First();
            var model = await _storageService
                .SaveToOSS(file, Convert.ToInt32(_configuration["ColossusPublicBucketId"]), 30);
            var record = new UploadRecord
            {
                UploaderId = user.Id,
                FileId = model.FileKey,
                SourceFileName = file.FileName.Replace(" ", "")
            };
            _dbContext.UploadRecords.Add(record);
            await _dbContext.SaveChangesAsync();
            return Json(new
            {
                message = "Uploaded!",
                value = model.Path
            });
        }

        public async Task<IActionResult> Logs()
        {
            var user = await GetCurrentUserAsync();
            var myFiles = await _dbContext
                .UploadRecords
                .Where(t => t.UploaderId == user.Id)
                .OrderByDescending(t => t.UploadTime)
                .ToListAsync();
            var accessToken = await _appsContainer.AccessToken();
            var colossusFiles = await _ossApiService.ViewAllFilesAsync(accessToken, Convert.ToInt32(_configuration["ColossusPublicBucketId"]));
            var myfilesOnOSS = colossusFiles.AllFiles.Where(t => myFiles.Any(k => k.FileId == t.FileKey));
            var model = new LogsViewModel(user, 1, "File upload logs")
            {
                Files = myfilesOnOSS,
                Records = myFiles
            };
            return View(model);
        }

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
