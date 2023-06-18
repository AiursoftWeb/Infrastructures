using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Portal.Controllers;

public class PortalController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}