using Aiursoft.Archon.SDK.Services;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Pylon;
using Aiursoft.SDK.Models.API.AccountAddressModels;
using Aiursoft.SDK.Models.API.AccountViewModels;
using Aiursoft.SDK.Services.ToDeveloperServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class AccountController : Controller
    {
        private readonly ACTokenManager _tokenManager;
        private readonly DeveloperApiService _apiService;
        private readonly GatewayDbContext _dbContext;
        private readonly UserAppAuthManager _authManager;

        public AccountController(
            ACTokenManager tokenManager,
            DeveloperApiService apiService,
            GatewayDbContext dbContext,
            UserAppAuthManager authManager)
        {
            _tokenManager = tokenManager;
            _apiService = apiService;
            _dbContext = dbContext;
            _authManager = authManager;
        }

        [APIProduces(typeof(CodeToOpenIdViewModel))]
        public async Task<IActionResult> CodeToOpenId(CodeToOpenIdAddressModel model)
        {
            var appId = await _tokenManager.ValidateAccessToken(model.AccessToken);
            var targetPack = await _dbContext
                .OAuthPack
                .SingleOrDefaultAsync(t => t.Code == model.Code);

            if (targetPack == null)
            {
                return this.Protocol(ErrorType.WrongKey, "The code doesn't exists in our database.");
            }
            // Use time is more than 10 seconds from now.
            if (targetPack.UseTime != DateTime.MinValue && targetPack.UseTime + TimeSpan.FromSeconds(10) < DateTime.UtcNow)
            {
                return this.Protocol(ErrorType.HasDoneAlready, "Code is used already!");
            }
            if (targetPack.ApplyAppId != appId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app granted code is not the app granting access token!");
            }
            var capp = (await _apiService.AppInfoAsync(targetPack.ApplyAppId)).App;
            if (!capp.ViewOpenId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app doesn't have view open id permission.");
            }
            targetPack.UseTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            var viewModel = new CodeToOpenIdViewModel
            {
                OpenId = targetPack.UserId,
                Scope = "scope",
                Message = "Successfully get user openid",
                Code = ErrorType.Success
            };
            return Json(viewModel);
        }

        [APIProduces(typeof(UserInfoViewModel))]
        public async Task<IActionResult> UserInfo(UserInfoAddressModel model)
        {
            var appId = await _tokenManager.ValidateAccessToken(model.AccessToken);
            var user = await _dbContext.Users.Include(t => t.Emails).SingleOrDefaultAsync(t => t.Id == model.OpenId);
            if (user == null)
            {
                return this.Protocol(ErrorType.NotFound, "Can not find a user with open id: " + model.OpenId);
            }
            if (!await _authManager.HasAuthorizedApp(user, appId))
            {
                return this.Protocol(ErrorType.Unauthorized, "The user did not allow your app to view his personal info! App Id: " + model.OpenId);
            }
            var viewModel = new UserInfoViewModel
            {
                Code = 0,
                Message = "Successfully get target user info.",
                User = user
            };
            return Json(viewModel);
        }
    }
}
