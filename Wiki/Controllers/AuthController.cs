using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.SDK.Models.ForApps.AddressModels;
using Aiursoft.SDK.Services;
using Aiursoft.Wiki.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Wiki.Controllers
{
    [LimitPerMin]
    public class AuthController : Controller
    {
        private readonly AuthService<WikiUser> _authService;
        public AuthController(
            AuthService<WikiUser> authService)
        {
            _authService = authService;
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