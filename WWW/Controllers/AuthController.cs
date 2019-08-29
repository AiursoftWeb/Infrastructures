using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon.Services;
using Aiursoft.WWW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Controllers
{
    [LimitPerMin]
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
            return RedirectToAction("Index", "Home");
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
            return Redirect(model.State);
        }
    }
}