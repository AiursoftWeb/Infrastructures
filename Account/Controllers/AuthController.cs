using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Account.Models;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;

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
            return Redirect(model.state);
        }
    }
}