using Aiursoft.Portal.Models;
using Aiursoft.Directory.SDK.Models.ForApps.AddressModels;

using Aiursoft.Identity.Attributes;
using Aiursoft.Identity.Services;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Portal.Controllers;


public class AuthController : Controller
{
    private readonly AuthService<PortalUser> _authService;

    public AuthController(
        AuthService<PortalUser> authService)
    {
        _authService = authService;
    }

    [AiurForceAuth("", "", false)]
    public IActionResult GoAuth()
    {
        return RedirectToAction("Index", "Home");
    }

    [AiurForceAuth("", "", false, true)]
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