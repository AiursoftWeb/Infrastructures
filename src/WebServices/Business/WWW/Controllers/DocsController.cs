using Aiursoft.Handler.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Controllers;

[LimitPerMin]
public class DocsController : Controller
{
    public IActionResult Terms()
    {
        return View();
    }
}