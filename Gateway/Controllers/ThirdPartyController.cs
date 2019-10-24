using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Models.ThirdPartyAddressModels;
using Aiursoft.Gateway.Models.ThirdyPartyViewModels;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
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
    [GenerateDoc]
    [LimitPerMin]
    [Route("Third-party")]
    [APINotfoundHandler]
    public class ThirdPartyController : Controller
    {
        private readonly IEnumerable<IAuthProvider> _authProviders;
        private readonly GatewayDbContext _dbContext;
        private readonly UserAppAuthManager _authManager;
        private readonly DeveloperApiService _apiService;
        private readonly UserManager<GatewayUser> _userManager;
        private readonly SignInManager<GatewayUser> _signInManager;
        private readonly AuthLogger _authLogger;
        private readonly ServiceLocation _serviceLocation;

        public ThirdPartyController(
            IEnumerable<IAuthProvider> authProviders,
            GatewayDbContext dbContext,
            UserAppAuthManager authManager,
            DeveloperApiService apiService,
            UserManager<GatewayUser> userManager,
            SignInManager<GatewayUser> signInManager,
            AuthLogger authLogger,
            ServiceLocation serviceLocation)
        {
            _authProviders = authProviders;
            _dbContext = dbContext;
            _authManager = authManager;
            _apiService = apiService;
            _userManager = userManager;
            _signInManager = signInManager;
            _authLogger = authLogger;
            _serviceLocation = serviceLocation;
        }

        [Route("sign-in/{providerName}")]
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
                    RedirectUri = oauthModel.RedirectUri,
                    State = oauthModel.State,
                }));
                return Redirect(refreshlink);
            }
            var account = await _dbContext
                .ThirdPartyAccounts
                .Include(t => t.Owner)
                .ThenInclude(t => t.Emails)
                .SingleOrDefaultAsync(t => t.OpenId == info.Id);
            var app = (await _apiService.AppInfoAsync(oauthModel.AppId)).App;
            if (account != null)
            {
                await _signInManager.SignInAsync(account.Owner, true);
                return await _authManager.FinishAuth(account.Owner, oauthModel, app.ForceConfirmation);
            }
            var viewModel = new SignInViewModel
            {
                RedirectUri = oauthModel.RedirectUri,
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
            if (string.IsNullOrWhiteSpace(model.UserDetail.Email))
            {
                model.UserDetail.Email = model.UserDetail.Name + $"@from.{model.ProviderName}.com";
            }
            bool exists = _dbContext.UserEmails.Any(t => t.EmailAddress == model.UserDetail.Email.ToLower());
            if (exists)
            {
                // TODO: Handle.
                throw new AiurAPIModelException(ErrorType.HasDoneAlready, $"An user with email '{model.UserDetail.Email}' already exists!");
            }
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            var user = new GatewayUser
            {
                UserName = model.UserDetail.Email + $".from.{model.ProviderName}.com",
                Email = model.UserDetail.Email,
                NickName = model.UserDetail.Name,
                PreferedLanguage = model.PreferedLanguage,
                IconFilePath = Values.DefaultImagePath,
                RegisterIPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
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
                    OpenId = model.UserDetail.Id
                };
                _dbContext.ThirdPartyAccounts.Add(link);
                await _dbContext.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: true);
                await _authLogger.LogAuthRecord(user.Id, HttpContext.Connection.RemoteIpAddress.ToString(), true, model.AppId);
                return await _authManager.FinishAuth(user, model, app.ForceConfirmation);
            }
            else
            {
                // TODO: Handle
                throw new AiurAPIModelException(ErrorType.HasDoneAlready, result.Errors.First().Description);
            }
        }

        [Route("bind-account/{providerName}")]
        public async Task<IActionResult> BindAccount(BindAccountAddressModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Redirect(_serviceLocation.Account + "/Auth/GoAuth");
            }
            if (user.ThirdPartyAccounts.Any(t => t.ProviderName == model.ProviderName))
            {
                var toDelete = await _dbContext.ThirdPartyAccounts
                    .Where(t => t.OwnerId == user.Id)
                    .Where(t => t.ProviderName == model.ProviderName)
                    .ToListAsync();
                _dbContext.ThirdPartyAccounts.RemoveRange(toDelete);
                await _dbContext.SaveChangesAsync();
            }
            var provider = _authProviders.SingleOrDefault(t => t.GetName().ToLower() == model.ProviderName.ToLower());
            if (provider == null)
            {
                // TODO: Handle.
                throw new NotImplementedException($"Provider: '{model.ProviderName}' is not implemented!");
            }
            IUserDetail info;
            try
            {
                info = await provider.GetUserDetail(model.Code, true);
            }
            catch (AiurAPIModelException)
            {
                var refreshlink = provider.GetBindRedirectLink();
                return Redirect(refreshlink);
            }
            var link = new ThirdPartyAccount
            {
                OwnerId = user.Id,
                OpenId = info.Id,
                ProviderName = provider.GetName()
            };
            _dbContext.ThirdPartyAccounts.Add(link);
            await _dbContext.SaveChangesAsync();
            // Complete
            var viewModel = new BindAccountViewModel
            {
                UserDetail = info,
                Provider = provider,
                User = user
            };
            return View(viewModel);
        }

        private Task<GatewayUser> GetCurrentUserAsync()
        {
            return _dbContext
                .Users
                .Include(t => t.ThirdPartyAccounts)
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}
