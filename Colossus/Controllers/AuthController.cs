using Aiursoft.Colossus.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.SDK.Models.ForApps.AddressModels;
using Aiursoft.SDK.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Controllers
{
    [LimitPerMin]
    public class AuthController : Controller
    {
        private readonly AuthService<ColossusUser> _authService;
        public AuthController(
            AuthService<ColossusUser> authService)
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