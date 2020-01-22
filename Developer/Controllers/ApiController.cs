using Aiursoft.Developer.Data;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.SDK.Models.Developer.ApiAddressModels;
using Aiursoft.SDK.Models.Developer.ApiViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class ApiController : Controller
    {
        private readonly DeveloperDbContext _dbContext;
        private readonly AiurCache _cache;

        public ApiController(
            DeveloperDbContext context,
            AiurCache cache)
        {
            _dbContext = context;
            _cache = cache;
        }

        public async Task<JsonResult> IsValidApp(IsValidateAppAddressModel model)
        {
            var target = await _cache.GetAndCache(model.AppId, async () => await _dbContext.Apps.FindAsync(model.AppId));
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

        [APIProduces(typeof(AppInfoViewModel))]
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