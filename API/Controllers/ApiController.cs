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
using Aiursoft.API.Models;
using Aiursoft.API.Services;
using Aiursoft.API.Data;
using Aiursoft.API.Models.ApiViewModels;
using Aiursoft.Pylon.Exceptions;

namespace Aiursoft.API.Controllers
{
    public class ApiController : Controller
    {
        private readonly UserManager<APIUser> _userManager;
        private readonly SignInManager<APIUser> _signInManager;
        private readonly ILogger _logger;
        private readonly APIDbContext _dbContext;
        private readonly IStringLocalizer<ApiController> _localizer;
        private readonly DeveloperApiService _developerApiService;

        public ApiController(
            UserManager<APIUser> userManager,
            SignInManager<APIUser> signInManager,
            ILoggerFactory loggerFactory,
            APIDbContext _context,
            IStringLocalizer<ApiController> localizer,
            DeveloperApiService developerApiService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                });
        }

        [HttpGet]
        public IActionResult Setlang(SetlangAddressModel model)
        {
            return View(new SetlangViewModel
            {
                Host = model.Host,
                Path = model.Path
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetLang(SetlangViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                _ApplyCultureCookie(model.Culture);
            }
            catch (CultureNotFoundException)
            {
                return Json(new AiurProtocal { Message = "Not a language.", Code = ErrorType.InvalidInput });
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                user.PreferedLanguage = model.Culture;
                await _userManager.UpdateAsync(user);
            }
            string toGo = new AiurUrl(model.Host, "Api", "SetSonLang", new
            {
                model.Culture,
                ReturnUrl = model.Path
            }).ToString();
            return Redirect(toGo);
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<JsonResult> ValidateAccessToken(string accessToken)
        {
            var target = await _dbContext.AccessToken
                .SingleOrDefaultAsync(t => t.Value == accessToken);
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
            try
            {
                await _developerApiService.IsValidAppAsync(model.AppId, model.AppSecret);
            }
            catch (AiurUnexceptedResponse e)
            {
                return Json(e.Response);
            }
            var newAC = new AccessToken
            {
                ApplyAppId = model.AppId,
                Value = Guid.NewGuid().ToString("N")
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
        public async Task<IActionResult> AllUserGranted(string accessToken)
        {
            var target = await _dbContext.AccessToken
                .SingleOrDefaultAsync(t => t.Value == accessToken);
            if (target == null || !target.IsAlive)
                return this.Protocal(ErrorType.Unauthorized, "We can not validate your access token!");
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
        public async Task<IActionResult> DropGrants(string accessToken)
        {
            var target = await _dbContext.AccessToken
                .SingleOrDefaultAsync(t => t.Value == accessToken);
            if (target == null || !target.IsAlive)
                return this.Protocal(ErrorType.Unauthorized, "We can not validate your access token!");
            _dbContext.LocalAppGrant.Delete(t => t.AppID == target.ApplyAppId);
            await _dbContext.SaveChangesAsync();
            return this.Protocal(ErrorType.Success, "Successfully droped all users granted!");
        }

        private async Task<APIUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}