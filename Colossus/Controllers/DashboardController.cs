using Aiursoft.Colossus.Data;
using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.DashboardViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToOSSServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Controllers
{
    [LimitPerMin]
    [AiurForceAuth]
    public class DashboardController : Controller
    {
        private readonly UserManager<ColossusUser> _userManager;
        private readonly ColossusDbContext _dbContext;
        private readonly OSSApiService _ossApiService;
        private readonly IConfiguration _configuration;
        private readonly StorageService _storageService;

        public DashboardController(
            UserManager<ColossusUser> userManager,
            ColossusDbContext dbContext,
            OSSApiService ossApiService,
            IConfiguration configuration,
            StorageService storageService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _ossApiService = ossApiService;
            _configuration = configuration;
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user, 0, "Upload new")
            {
                MaxSize = int.MaxValue
            };
            return View(model);
        }

        [Obsolete]
        [HttpPost]
        [APIExpHandler]
        [FileChecker]
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
            return Json(new AiurValue<string>(model.Path)
            {
                Code = ErrorType.Success,
                Message = "Uploaded!"
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
            var myfilesOnOSS = await _ossApiService.ViewMultiFilesAsync(myFiles.Select(t => t.FileId).ToArray());

            // find all out-dated records.
            var outdatedRecords = myFiles.Where(t => myfilesOnOSS.Items.Any(p => p.FileKey == t.FileId) == false).ToList();
            if (outdatedRecords.Any())
            {
                _dbContext.UploadRecords.RemoveRange(outdatedRecords);
                await _dbContext.SaveChangesAsync();
            }
            var model = new LogsViewModel(user, 1, "File upload logs")
            {
                Files = myfilesOnOSS.Items,
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
