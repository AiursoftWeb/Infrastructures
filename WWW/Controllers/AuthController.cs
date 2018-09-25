using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Aiursoft.WWW.Models;
using Aiursoft.WWW.Data;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Attributes;
using System;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.WWW.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService<WWWUser> _authService;
        private readonly UserManager<WWWUser> _userManager;

        public AuthController(
            AuthService<WWWUser> authService,
            UserManager<WWWUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [AiurForceAuth(preferController: "", preferAction: "", justTry: false, register: false)]
        public IActionResult GoAuth()
        {
            throw new NotImplementedException();
        }
        [AiurForceAuth(preferController: "", preferAction: "", justTry: false, register: true)]
        public IActionResult GoRegister()
        {
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> AuthResult(AuthResultAddressModel model)
        {
            var user = await _authService.AuthApp(model);
            this.SetClientLang(user.PreferedLanguage);
            return Redirect(model.state);
        }

        [AiurForceAuth(directlyReject: true)]
        [APIExpHandler]
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var updated = await _authService.OnlyUpdate(user);
            return Json(new AiurValue<WWWUser>(updated)
            {
                Code = ErrorType.Success,
                Message = "Server database synced with API!"
            });
        }
    }
}