using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.ToOSSServer;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Aiursoft.Developer.Models.FilesViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    public class FilesController : Controller
    {
        public readonly UserManager<DeveloperUser> _userManager;
        public readonly SignInManager<DeveloperUser> _signInManager;
        public readonly ILogger _logger;
        public DeveloperDbContext _dbContext;

        public FilesController(
        UserManager<DeveloperUser> userManager,
        SignInManager<DeveloperUser> signInManager,
        ILoggerFactory loggerFactory,
        DeveloperDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<FilesController>();
            _dbContext = _context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        public async Task<IActionResult> ViewFiles(int id)//Bucket Id
        {
            var cuser = await GetCurrentUserAsync();
            var bucketInfo = await ApiService.ViewBucketDetailAsync(id);
            if (bucketInfo.BelongingAppId == null)
            {
                return NotFound();
            }
            var app = await _dbContext.Apps.FindAsync(bucketInfo.BelongingAppId);
            var files = await ApiService.ViewAllFilesAsync(await AppsContainer.AccessToken(app.AppId, app.AppSecret)(), id);
            var model = new ViewFilesViewModel(cuser)
            {
                BucketId = files.BucketId,
                AllFiles = files.AllFiles,
                AppId = app.AppId,
                OpenToRead = bucketInfo.OpenToRead,
                BucketName = bucketInfo.BucketName
            };
            return View(model);
        }

        public async Task<IActionResult> GenerateLink(int id)
        {
            var cuser = await GetCurrentUserAsync();
            var fileinfo = await ApiService.ViewOneFileAsync(id);
            if (fileinfo.File == null)
            {
                return NotFound();
            }
            var bucketInfo = await ApiService.ViewBucketDetailAsync(fileinfo.File.BucketId);
            var app = await _dbContext.Apps.FindAsync(bucketInfo.BelongingAppId);
            if (bucketInfo.BelongingAppId != app.AppId)
            {
                return Unauthorized();
            }
            var secret = await SecretService.GenerateAsync(id, await AppsContainer.AccessToken(app.AppId, app.AppSecret)());
            var model = new GenerateLinkViewModel(cuser)
            {
                Address = secret.Value,
                BucketId = bucketInfo.BucketId
            };
            return View(model);
        }

        public async Task<IActionResult> DeleteFile(int id)
        {
            var cuser = await GetCurrentUserAsync();
            var fileinfo = await ApiService.ViewOneFileAsync(id);
            var bucketInfo = await ApiService.ViewBucketDetailAsync(fileinfo.File.BucketId);
            var app = await _dbContext.Apps.FindAsync(bucketInfo.BelongingAppId);

            if (bucketInfo.BelongingAppId != app.AppId)
            {
                return Unauthorized();
            }
            var model = new DeleteFileViewModel(cuser)
            {
                FileName = fileinfo.File.RealFileName,
                FileId = fileinfo.File.FileKey,
                BucketId = fileinfo.File.BucketId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(DeleteFileViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(cuser, 3);
                return View(model);
            }
            var fileinfo = await ApiService.ViewOneFileAsync(model.FileId);
            var bucketInfo = await ApiService.ViewBucketDetailAsync(fileinfo.File.BucketId);
            var app = await _dbContext.Apps.FindAsync(bucketInfo.BelongingAppId);

            if (fileinfo == null || bucketInfo.BelongingAppId != app.AppId || fileinfo.File.BucketId != bucketInfo.BucketId)
            {
                return Unauthorized();
            }
            await ApiService.DeleteFileAsync(await AppsContainer.AccessToken(app.AppId, app.AppSecret)(), model.FileId, bucketInfo.BucketId);
            return RedirectToAction(nameof(ViewFiles), new
            {
                id = bucketInfo.BucketId
            });
        }
        public async Task<IActionResult> UploadFile(int id)//BucketId
        {
            var cuser = await GetCurrentUserAsync();
            var bucket = await ApiService.ViewBucketDetailAsync(id);
            var viewModel = new UploadFileViewModel(cuser)
            {
                BucketId = bucket.BucketId,
                AppId = bucket.BelongingAppId,
                BucketName = bucket.BucketName,
            };
            return View(viewModel);
        }

        [HttpPost]
        [FileChecker]
        public async Task<IActionResult> UploadFile(UploadFileViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(cuser, 3);
                model.ModelStateValid = false;
                return View(model);
            }
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            string accessToken = await AppsContainer.AccessToken(app.AppId, app.AppSecret)();
            var file = Request.Form.Files.First();
            await StorageService.SaveToOSS(file, model.BucketId, model.AliveDays, SaveFileOptions.SourceName, accessToken);
            return RedirectToAction(nameof(ViewFiles), new { id = model.BucketId });
        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}
