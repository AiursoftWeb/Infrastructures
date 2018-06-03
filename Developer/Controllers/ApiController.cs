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

namespace Aiursoft.Developer.Controllers
{
    public class ApiController : Controller
    {
        public readonly UserManager<DeveloperUser> _userManager;
        public readonly SignInManager<DeveloperUser> _signInManager;
        public readonly ILogger _logger;
        public DeveloperDbContext _dbContext;

        public ApiController(
        UserManager<DeveloperUser> userManager,
        SignInManager<DeveloperUser> signInManager,
        ILoggerFactory loggerFactory,
        DeveloperDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<ApiController>();
            _dbContext = _context;
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<JsonResult> IsValidApp(IsValidateAppAddressModel model)
        {
            var _target = await _dbContext.Apps.FindAsync(model.AppId);
            if (_target == null)
            {
                return Json(new AiurProtocal { Message = "Target app did not found.", Code = ErrorType.NotFound });
            }
            else if (_target.AppSecret != model.AppSecret)
            {
                return Json(new AiurProtocal { Message = "Wrong secret.", Code = ErrorType.WrongKey });
            }
            else
            {
                return Json(new AiurProtocal { Message = "Correct app info.", Code = ErrorType.Success });
            }
        }
        
        //http://developer.aiursoft.obisoft.com.cn/api/AppInfo?AppId=29bf5250a6d93d47b6164ac2821d5009
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<JsonResult> AppInfo(AppInfoAddressModel model)
        {
            var target = await _dbContext
                .Apps
                .SingleOrDefaultAsync(t => t.AppId == model.AppId);

            if (target == null)
            {
                return Json(new AiurProtocal { Message = "Not found.", Code = ErrorType.NotFound });
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