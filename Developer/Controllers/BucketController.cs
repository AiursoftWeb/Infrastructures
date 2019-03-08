using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models.BucketViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.OSS;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToOSSServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    public class BucketController : Controller
    {
        private readonly UserManager<DeveloperUser> _userManager;
        private readonly SignInManager<DeveloperUser> _signInManager;
        private readonly ILogger _logger;
        private readonly DeveloperDbContext _dbContext;
        private readonly AppsContainer _appsContainer;
        private readonly OSSApiService _ossApiService;

        public BucketController(
            UserManager<DeveloperUser> userManager,
            SignInManager<DeveloperUser> signInManager,
            ILoggerFactory loggerFactory,
            DeveloperDbContext dbContext,
            AppsContainer appsContainer,
            OSSApiService ossApiService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<BucketController>();
            _dbContext = dbContext;
            _appsContainer = appsContainer;
            _ossApiService = ossApiService;
        }

        public async Task<IActionResult> Index()
        {
            var cuser = await GetCurrentUserAsync();
            var allBuckets = new List<Bucket>();
            var taskList = new List<Task>();
            foreach (var app in cuser.MyApps)
            {
                async Task addApp()
                {
                    var appInfo = await _ossApiService.ViewMyBucketsAsync(await _appsContainer.AccessToken(app.AppId, app.AppSecret));
                    allBuckets.AddRange(appInfo.Buckets);
                };
                taskList.Add(addApp());
            }
            await Task.WhenAll(taskList);
            var model = new IndexViewModel(cuser)
            {
                AllBuckets = allBuckets.GroupBy(t => t.BelongingAppId).OrderBy(t => t.Key)
            };
            return View(model);
        }

        public async Task<IActionResult> CreateBucket(string id)//AppId
        {
            var cuser = await GetCurrentUserAsync();
            var viewModel = new CreateBucketViewModel(this, cuser)
            {
                AppId = id,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBucket([FromForm]CreateBucketViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(this, cuser);
                return View(model);
            }
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            if (app == null)
            {
                return NotFound();
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                var result = await _ossApiService.CreateBucketAsync(token, model.NewBucketName, model.OpenToRead, model.OpenToUpload);
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = app.AppId, JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(this, cuser);
                return View(model);
            }
        }

        public async Task<IActionResult> EditBucket(int id)//BucketId
        {
            var cuser = await GetCurrentUserAsync();
            var bucket = await _ossApiService.ViewBucketDetailAsync(id);
            var model = new EditBucketViewModel(cuser, bucket)
            {
                AppId = bucket.BelongingAppId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBucket(EditBucketViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser, 1);
                return View(model);
            }
            try
            {
                var app = await _dbContext.Apps.FindAsync(model.AppId);
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                var bucket = await _ossApiService.ViewBucketDetailAsync(model.BucketId);
                if (bucket.BelongingAppId != app.AppId || app.CreatorId != cuser.Id)
                    return Unauthorized();
                await _ossApiService.EditBucketAsync(token, model.BucketId, model.NewBucketName, model.OpenToRead, model.OpenToUpload);
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = model.AppId, JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(cuser, 1);
                return View(model);
            }
        }

        public async Task<IActionResult> DeleteBucket(int id)//BucketId
        {
            var cuser = await GetCurrentUserAsync();
            var bucket = await _ossApiService.ViewBucketDetailAsync(id);
            var model = new DeleteBucketViewModel(cuser)
            {
                BucketName = bucket.BucketName,
                FilesCount = bucket.FileCount,
                AppId = bucket.BelongingAppId,
                BucketId = bucket.BucketId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBucket([FromForm]DeleteBucketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var app = await _dbContext.Apps.FindAsync(model.AppId);
                var cuser = await GetCurrentUserAsync();
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                var bucket = await _ossApiService.ViewBucketDetailAsync(model.BucketId);
                if (bucket.BelongingAppId != app.AppId || app.CreatorId != cuser.Id)
                {
                    return Unauthorized();
                }
                await _ossApiService.DeleteBucketAsync(token, model.BucketId);
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = model.AppId });
            }
            return View(model);
        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}
