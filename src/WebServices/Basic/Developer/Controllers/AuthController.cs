using Aiursoft.Developer.Models;
using Aiursoft.Gateway.SDK.Models.ForApps.AddressModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity.Attributes;
using Aiursoft.Identity.Services;
using Aiursoft.SDK;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [LimitPerMin]
    public class AuthController : Controller
    {
        private readonly AuthService<DeveloperUser> _authService;
        public AuthController(
            AuthService<DeveloperUser> authService)
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