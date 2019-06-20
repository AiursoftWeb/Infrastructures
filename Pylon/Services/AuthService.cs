using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class AuthService<T> where T : AiurUserBase, new()
    {
        private readonly UserManager<T> _userManager;
        private readonly SignInManager<T> _signInManager;
        private readonly AccountService _accountService;
        private readonly AppsContainer _appsContainer;
        public AuthService(
            UserManager<T> userManager,
            SignInManager<T> signInManager,
            AccountService accountService,
            AppsContainer appsContainer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _appsContainer = appsContainer;
        }

        public async Task<T> AuthApp(AuthResultAddressModel model, bool isPersistent = false)
        {
            var openId = await _accountService.CodeToOpenIdAsync(await _appsContainer.AccessToken(), model.Code);
            var userinfo = await _accountService.OpenIdToUserInfo(await _appsContainer.AccessToken(), openId.OpenId);
            var current = await _userManager.FindByIdAsync(userinfo.User.Id);
            if (current == null)
            {
                current = new T();
                current.Update(userinfo);
                var result = await _userManager.CreateAsync(current);
                if (!result.Succeeded)
                {
                    var message = new StringBuilder();
                    foreach (var error in result.Errors)
                    {
                        message.AppendLine(error.Description);
                    }
                    throw new InvalidOperationException($"The user info ({userinfo.User.Id}) we get could not register to our database because {message}.");
                }
            }
            else
            {
                current.Update(userinfo);
                await _userManager.UpdateAsync(current);
            }
            await _signInManager.SignInAsync(current, isPersistent);
            return current;
        }

        public async Task<T> OnlyUpdate(T user)
        {
            var userinfo = await _accountService.OpenIdToUserInfo(accessToken: await _appsContainer.AccessToken(), openid: user.Id);
            user.Update(userinfo);
            await _userManager.UpdateAsync(user);
            return user;
        }
    }
}
