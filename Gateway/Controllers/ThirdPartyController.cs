using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models.ThirdPartyAddressModels;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.Authentication;
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

        public ThirdPartyController(
            IEnumerable<IAuthProvider> authProviders,
            GatewayDbContext dbContext,
            AuthFinisher authFinisher)
        {
            _authProviders = authProviders;
            _dbContext = dbContext;
            _authFinisher = authFinisher;
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
            var info = await provider.GetUserDetail(model.Code);
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
            return Json(info);
        }
    }
}
