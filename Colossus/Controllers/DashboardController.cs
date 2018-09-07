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

        public DashboardController(
            UserManager<ColossusUser> userManager,
            ColossusDbContext dbContext,
            OSSApiService ossApiService,
            IConfiguration configuration,
            AppsContainer appsContainer)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _ossApiService = ossApiService;
            _configuration = configuration;
            _appsContainer = appsContainer;
        }

        public async Task<IActionResult> Index()
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
            var model = new IndexViewModel(user, 0, "File upload history")
            {
                Files = myfilesOnOSS
            };
            return View(model);
        }

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            if (User.Identity.Name == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(User.Identity.Name);
        }
    }
}
