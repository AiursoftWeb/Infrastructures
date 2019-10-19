using Aiursoft.Gateway.Controllers;
using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthViewModels;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class AuthFinisher : IScopedDependency
    {
        private readonly GatewayDbContext _dbContext;

        public AuthFinisher(GatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> FinishAuth(Controller controller, IAuthorizeViewModel model, bool forceGrant = false)
        {
            var user = await GetUserFromEmail(model.Email);
            if (await user.HasAuthorizedApp(_dbContext, model.AppId) && forceGrant == false)
            {
                var pack = await user.GeneratePack(_dbContext, model.AppId);
                var url = new AiurUrl(model.GetRegexRedirectUrl(), new AuthResultAddressModel
                {
                    Code = pack.Code,
                    State = model.State
                });
                return controller.Redirect(url.ToString());
            }
            else
            {
                return controller.RedirectToAction(nameof(OAuthController.AuthorizeConfirm), new AuthorizeConfirmAddressModel
                {
                    AppId = model.AppId,
                    State = model.State,
                    ToRedirect = model.ToRedirect,
                    Scope = model.Scope,
                    ResponseType = model.ResponseType
                });
            }
        }

        private Task<GatewayUser> GetUserFromEmail(string email)
        {
            return _dbContext
                .Users
                .Include(t => t.Emails)
                .SingleOrDefaultAsync(t => t.Emails.Any(p => p.EmailAddress == email));
        }
    }
}
