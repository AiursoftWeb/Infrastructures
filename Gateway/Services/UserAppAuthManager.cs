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
    public class UserAppAuthManager : IScopedDependency
    {
        private readonly GatewayDbContext _dbContext;

        public UserAppAuthManager(GatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> FinishAuth(GatewayUser user, FinishAuthInfo model, bool forceGrant = false)
        {
            if (await HasAuthorizedApp(user, model.AppId) && forceGrant == false)
            {
                var pack = await GeneratePack(user, model.AppId);
                var url = new AiurUrl(GetRegexRedirectUri(model.RedirectUri), new AuthResultAddressModel
                {
                    Code = pack.Code,
                    State = model.State
                });
                return new RedirectResult(url.ToString());
            }
            else
            {
                return new RedirectToActionResult(nameof(OAuthController.AuthorizeConfirm), "OAuth", model);
            }
        }


        public async Task GrantTargetApp(GatewayUser user, string appId)
        {
            if (!await HasAuthorizedApp(user, appId))
            {
                var appGrant = new AppGrant
                {
                    AppID = appId,
                    GatewayUserId = user.Id
                };
                _dbContext.LocalAppGrant.Add(appGrant);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<OAuthPack> GeneratePack(GatewayUser user, string appId)
        {
            var pack = new OAuthPack
            {
                Code = Math.Abs(Guid.NewGuid().GetHashCode()),
                UserId = user.Id,
                ApplyAppId = appId
            };
            _dbContext.OAuthPack.Add(pack);
            await _dbContext.SaveChangesAsync();
            return pack;
        }

        public async Task<bool> HasAuthorizedApp(GatewayUser user, string appId)
        {
            return await _dbContext.LocalAppGrant.AnyAsync(t => t.AppID == appId && t.GatewayUserId == user.Id);
        }

        private string GetRegexRedirectUri(string sourceUrl)
        {
            var url = new Uri(sourceUrl);
            return $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
        }
    }
}
