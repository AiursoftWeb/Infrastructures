using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Code404()
        {
            return View();
        }
        public IActionResult ServerException()
        {
            return View();
        }
    }
}
