using Aiursoft.Handler.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Controllers;


public class DocsController : Controller
{
    public IActionResult Terms()
    {
        return View();
    }
}