using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW
{
    [LimitPerMin]
    public class DocsController : Controller
    {
        public IActionResult Terms()
        {
            return View();
        }
    }
}