using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Aiursoft.Pylon.Models.API.ApiViewModels;
using Aiursoft.Pylon.Models.API.ApiAddressModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models.API;
using Aiursoft.API.Attributes;
using Aiursoft.API.Models;
using Aiursoft.API.Services;
using Aiursoft.API.Data;
using Aiursoft.API.Models.ApiViewModels;

namespace Aiursoft.API.Controllers
{
    public class ApiController : Controller
    {
        private readonly ILogger _logger;
        private readonly APIDbContext _dbContext;
        private readonly IStringLocalizer<ApiController> _localizer;
        private readonly DeveloperApiService _developerApiService;

        public ApiController(
            ILoggerFactory loggerFactory,
            APIDbContext _context,
            IStringLocalizer<ApiController> localizer,
            DeveloperApiService developerApiService)
        {
            _logger = loggerFactory.CreateLogger<ApiController>();
            _dbContext = _context;
            _localizer = localizer;
            _developerApiService = developerApiService;
        }

        private void _ApplyCultureCookie(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        [HttpGet]
        public IActionResult Setlang(string host, string path)
        {
            return View(new SetlangViewModel
            {
                Host = host,
                Path = path
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetLang(string culture, string host, string path)
        {
            try
            {
                _ApplyCultureCookie(culture);
                var user = await GetCurrentUserAsync();
                if (user != null)
                {
                    user.PreferedLanguage = culture;
                    await _userManager.UpdateAsync(user);
                }
                string toGo = new AiurUrl(host, "Api", "SetSonLang", new
                {
                    Culture = culture,
                    ReturnUrl = path
                }).ToString();
                return Redirect(toGo);
            }
            catch (CultureNotFoundException)
            {
                return Json(new AiurProtocal { Message = "Not a language.", Code = ErrorType.InvalidInput });
            }
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<JsonResult> ValidateAccessToken(string AccessToken)
        {
            var target = await _dbContext.AccessToken
                .SingleOrDefaultAsync(t => t.Value == AccessToken);
            if (target == null)
            {
                return Json(new ValidateAccessTokenViewModel { Code = ErrorType.Unauthorized, Message = "We can not validate your access token!" });
            }
            else if (!target.IsAlive)
            {
                return Json(new ValidateAccessTokenViewModel { Code = ErrorType.Timeout, Message = "Your access token is already Timeout!" });
            }
            else
            {
                return Json(new ValidateAccessTokenViewModel
                {
                    AppId = target.ApplyAppId,
                    Code = ErrorType.Success,
                    Message = "Successfully validated access token."
                });
            }
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
        {
            var AppValidateState = await _developerApiService.IsValidAppAsync(model.AppId, model.AppSecret);
            if (AppValidateState.Code != ErrorType.Success)
            {
                return Json(AppValidateState);
            }
            var newAC = new AccessToken
            {
                ApplyAppId = model.AppId,
                Value = (DateTime.Now.ToString() + HttpContext.GetHashCode().ToString() + model.AppId).GetMD5()
            };
            _dbContext.AccessToken.Add(newAC);
            await _dbContext.SaveChangesAsync();
            return Json(new AccessTokenViewModel
            {
                Code = ErrorType.Success,
                Message = "Successfully get access token.",
                AccessToken = newAC.Value,
                DeadTime = newAC.CreateTime + newAC.AliveTime
            });
        }

        [APIExpHandler]
        [APIModelStateChecker]
        [ForceValidateAccessToken]
        public async Task<IActionResult> AllUserGranted(string AccessToken)
        {
            var target = await _dbContext.AccessToken
                .SingleOrDefaultAsync(t => t.Value == AccessToken);

            var grants = _dbContext.LocalAppGrant.Include(t => t.User).Where(t => t.AppID == target.ApplyAppId).Take(200);
            var model = new AllUserGrantedViewModel
            {
                AppId = target.ApplyAppId,
                Grants = new List<Grant>(),
                Code = ErrorType.Success,
                Message = "Successfully get all your users"
            };
            model.Grants.AddRange(grants);
            return Json(model);
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        [ForceValidateAccessToken]
        public async Task<IActionResult> DropGrants(string accessToken)
        {
            var target = await _dbContext.AccessToken
                .SingleOrDefaultAsync(t => t.Value == accessToken);
            _dbContext.LocalAppGrant.Delete(t => t.AppID == target.ApplyAppId);
            await _dbContext.SaveChangesAsync();
            return this.Protocal(ErrorType.Success, "Successfully droped all users granted!");
        }
    }
}