using Aiursoft.Account.Models;
using Aiursoft.Gateway.SDK.Models.ForApps.AddressModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity.Attributes;
using Aiursoft.Identity.Services;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Account.Controllers
{
    [LimitPerMin]
    public class AuthController : Controller
    {
        private readonly AuthService<AccountUser> _authService;
        public AuthController(
            AuthService<AccountUser> authService)
        {
            _authService = authService;
        }

        [AiurForceAuth(preferController: "", preferAction: "", justTry: false)]
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