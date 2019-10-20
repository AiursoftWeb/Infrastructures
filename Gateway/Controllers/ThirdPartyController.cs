using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Models.ThirdPartyAddressModels;
using Aiursoft.Gateway.Models.ThirdyPartyViewModels;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.Authentication;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Controllers
{
    [Route("Third-party")]
    public class ThirdPartyController : Controller
    {
        private readonly IEnumerable<IAuthProvider> _authProviders;
        private readonly GatewayDbContext _dbContext;
        private readonly UserAppAuthManager _authManager;
        private readonly DeveloperApiService _apiService;
        private readonly UserManager<GatewayUser> _userManager;
        private readonly SignInManager<GatewayUser> _signInManager;
        private readonly AuthLogger _authLogger;

        public ThirdPartyController(
            IEnumerable<IAuthProvider> authProviders,
            GatewayDbContext dbContext,
            UserAppAuthManager authManager,
            DeveloperApiService apiService,
            UserManager<GatewayUser> userManager,
            SignInManager<GatewayUser> signInManager,
            AuthLogger authLogger)
        {
            _authProviders = authProviders;
            _dbContext = dbContext;
            _authManager = authManager;
            _apiService = apiService;
            _userManager = userManager;
            _signInManager = signInManager;
            _authLogger = authLogger;
        }

        [Route("Sign-in/{providerName}")]
        public async Task<IActionResult> SignIn(SignInAddressModel model)
        {
            var provider = _authProviders.SingleOrDefault(t => t.GetName().ToLower() == model.ProviderName.ToLower());
            if (provider == null)
            {
                return NotFound();
            }
            var oauthModel = model.BuildOAuthInfo();
            IUserDetail info = null;
            try
            {
                info = await provider.GetUserDetail(model.Code);
            }
            catch (AiurAPIModelException)
            {
                var refreshlink = provider.GetSignInRedirectLink(new AiurUrl("", new FinishAuthInfo
                {
                    AppId = oauthModel.AppId,
                    RedirectUrl = oauthModel.RedirectUrl,
                    State = oauthModel.State,
                }));
                return Redirect(refreshlink);
            }
            var app = (await _apiService.AppInfoAsync(oauthModel.AppId)).App;
            var account = await _dbContext
                .ThirdPartyAccounts
                .Include(t => t.Owner)
                .ThenInclude(t => t.Emails)
                .SingleOrDefaultAsync(t => t.OpenId == info.Id.ToString());
            if (account != null)
            {
                await _authManager.FinishAuth(account.Owner, oauthModel);
            }
            var viewModel = new SignInViewModel
            {
                RedirectUrl = oauthModel.RedirectUrl,
                State = oauthModel.State,
                AppId = oauthModel.AppId,
                UserDetail = info,
                ProviderName = model.ProviderName,
                AppImageUrl = app.IconPath,
                CanFindAnAccountWithEmail = await _dbContext.UserEmails.AnyAsync(t => t.EmailAddress.ToLower() == info.Email.ToLower()),
                Provider = provider
            };
            return View(viewModel);
        }

        [HttpPost]
        [Route("create-account-and-bind/{providerName}")]
        public async Task<IActionResult> CreateAccountAndBind(SignInViewModel model)
        {
            bool exists = _dbContext.UserEmails.Any(t => t.EmailAddress == model.UserDetail.Email.ToLower());
            if (exists)
            {
                throw new AiurAPIModelException(ErrorType.HasDoneAlready, $"An user with email '{model.UserDetail.Email}' already exists!");
            }
            var user = new GatewayUser
            {
                UserName = model.UserDetail.Email + $".from.{model.ProviderName}.com",
                Email = model.UserDetail.Email,
                NickName = model.UserDetail.Name,
                PreferedLanguage = model.PreferedLanguage,
                IconFilePath = Values.DefaultImagePath
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var primaryMail = new UserEmail
                {
                    EmailAddress = model.UserDetail.Email.ToLower(),
                    OwnerId = user.Id,
                    ValidateToken = Guid.NewGuid().ToString("N")
                };
                _dbContext.UserEmails.Add(primaryMail);
                await _dbContext.SaveChangesAsync();

                var link = new ThirdPartyAccount
                {
                    OwnerId = user.Id,
                    ProviderName = model.ProviderName,
                    OpenId = model.UserDetail.Id.ToString()
                };
                _dbContext.ThirdPartyAccounts.Add(link);
                await _dbContext.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: true);
                await _authLogger.LogAuthRecord(user.Id, HttpContext.Connection.RemoteIpAddress.ToString(), true, model.AppId);
                return await _authManager.FinishAuth(user, model);
            }
            else
            {
                throw new AiurAPIModelException(ErrorType.HasDoneAlready, result.Errors.First().Description);
            }
        }
    }
}
