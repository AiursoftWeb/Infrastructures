using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.Developer.ApiAddressModels;
using Aiursoft.Pylon.Models.Developer.ApiViewModels;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Aiursoft.Developer.Controllers
{
    public class ApiController : Controller
    {
        private readonly UserManager<DeveloperUser> _userManager;
        private readonly SignInManager<DeveloperUser> _signInManager;
        private readonly ILogger _logger;
        private readonly DeveloperDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public ApiController(
        UserManager<DeveloperUser> userManager,
        SignInManager<DeveloperUser> signInManager,
        ILoggerFactory loggerFactory,
        DeveloperDbContext _context,
        IMemoryCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<ApiController>();
            _dbContext = _context;
            _cache = cache;
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<JsonResult> IsValidApp(IsValidateAppAddressModel model)
        {
            if (!_cache.TryGetValue(model.AppId, out App target))
            {
                target = await _dbContext.Apps.FindAsync(model.AppId);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(20));

                _cache.Set(model.AppId, target, cacheEntryOptions);
            }
            if (target == null)
            {
                return Json(new AiurProtocol { Message = "Target app did not found.", Code = ErrorType.NotFound });
            }
            else if (target.AppSecret != model.AppSecret)
            {
                return Json(new AiurProtocol { Message = "Wrong secret.", Code = ErrorType.WrongKey });
            }
            else
            {
                return Json(new AiurProtocol { Message = "Correct app info.", Code = ErrorType.Success });
            }
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<JsonResult> AppInfo(AppInfoAddressModel model)
        {
            var target = await _dbContext
                .Apps
                .SingleOrDefaultAsync(t => t.AppId == model.AppId);

            if (target == null)
            {
                return Json(new AiurProtocol { Message = $"Could find target app with appId: '{model.AppId}'!", Code = ErrorType.NotFound });
            }
            return Json(new AppInfoViewModel
            {
                Message = "Successfully get target app info.",
                Code = ErrorType.Success,
                App = target
            });
        }
    }
}