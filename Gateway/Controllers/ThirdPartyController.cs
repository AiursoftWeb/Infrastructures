using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models.ThirdPartyAddressModels;
using Aiursoft.Gateway.Models.ThirdyPartyViewModels;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.Authentication;
using Aiursoft.Pylon.Services.ToDeveloperServer;
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

        public ThirdPartyController(
            IEnumerable<IAuthProvider> authProviders,
            GatewayDbContext dbContext,
            AuthFinisher authFinisher,
            DeveloperApiService apiService)
        {
            _authProviders = authProviders;
            _dbContext = dbContext;
            _authFinisher = authFinisher;
            _apiService = apiService;
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

        [Route("create-account-and-bind/{providerName}")]
        public async Task<IActionResult> CreateAccountAndBind(SignInViewModel model)
        {
            await Task.Delay(0);
            throw new NotImplementedException();
        }
    }
}
