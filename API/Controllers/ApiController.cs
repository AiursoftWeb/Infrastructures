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
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Aiursoft.Pylon.Models.API.ApiViewModels;
using Aiursoft.Pylon.Models.API.ApiAddressModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models.API;
using Aiursoft.API.Models;
using Aiursoft.API.Data;
using Aiursoft.API.Models.ApiViewModels;
using Aiursoft.Pylon.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.API.Controllers
{
    public class ApiController : Controller
    {
        private readonly UserManager<APIUser> _userManager;
        private readonly APIDbContext _dbContext;
        private readonly DeveloperApiService _developerApiService;
        private readonly ACTokenManager _tokenManager;

        public ApiController(
            UserManager<APIUser> userManager,
            ILoggerFactory loggerFactory,
            APIDbContext context,
            DeveloperApiService developerApiService,
            ACTokenManager tokenManager)
        {
            _userManager = userManager;
            loggerFactory.CreateLogger<ApiController>();
            _dbContext = context;
            _developerApiService = developerApiService;
            _tokenManager = tokenManager;
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
                return Json(new AiurProtocol { Message = "Not a language.", Code = ErrorType.InvalidInput });
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
            var token = _tokenManager.GenerateAccessToken(model.AppId);
            return Json(new AccessTokenViewModel
            {
                Code = ErrorType.Success,
                Message = "Successfully get access token.",
                AccessToken = token.Item1,
                DeadTime = token.Item2
            });
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> AllUserGranted([Required]string accessToken)
        {
            var appid = _tokenManager.ValidateAccessToken(accessToken);
            var grants = await _dbContext.LocalAppGrant.Include(t => t.User).Where(t => t.AppID == appid).Take(200).ToListAsync();
            var model = new AllUserGrantedViewModel
            {
                AppId = appid,
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
        public async Task<IActionResult> DropGrants([Required]string accessToken)
        {
            var appid = _tokenManager.ValidateAccessToken(accessToken);
            _dbContext.LocalAppGrant.Delete(t => t.AppID == appid);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully droped all users granted!");
        }

        private Task<APIUser> GetCurrentUserAsync()
        {
            return _dbContext
                .Users
                .Include(t => t.Emails)
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}