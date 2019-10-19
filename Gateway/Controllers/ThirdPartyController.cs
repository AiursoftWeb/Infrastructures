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
        private readonly AuthFinisher _authFinisher;
        private readonly DeveloperApiService _apiService;
        private readonly UserManager<GatewayUser> _userManager;
        private readonly SignInManager<GatewayUser> _signInManager;

        public ThirdPartyController(
            IEnumerable<IAuthProvider> authProviders,
            GatewayDbContext dbContext,
            AuthFinisher authFinisher,
            DeveloperApiService apiService,
            UserManager<GatewayUser> userManager,
            SignInManager<GatewayUser> signInManager)
        {
            _authProviders = authProviders;
            _dbContext = dbContext;
            _authFinisher = authFinisher;
            _apiService = apiService;
            _userManager = userManager;
            _signInManager = signInManager;
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
                var refreshlink = provider.GetSignInRedirectLink(new AiurUrl("", new
                {
                    appid = oauthModel.AppId,
                    redirect_uri = oauthModel.ToRedirect,
                    state = oauthModel.State,
                    scope = oauthModel.Scope,
                    response_type = oauthModel.ResponseType
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
                oauthModel.Email = account.Owner.Email;
                await _authFinisher.FinishAuth(this, oauthModel);
            }
            var viewModel = new SignInViewModel
            {
                OAuthInfo = oauthModel,
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
                UserName = model.UserDetail.Email,
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
                await _signInManager.SignInAsync(user, isPersistent: true);
                var log = new AuditLogLocal
                {
                    UserId = user.Id,
                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    Success = true,
                    AppId = model.OAuthInfo.AppId
                };
                _dbContext.AuditLogs.Add(log);
                await _dbContext.SaveChangesAsync();
                return await _authFinisher.FinishAuth(this, model.OAuthInfo);
            }
            else
            {
                throw new AiurAPIModelException(ErrorType.HasDoneAlready, result.Errors.First().Description);
            }
        }
    }
}
