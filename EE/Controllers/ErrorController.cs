using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.EE.Controllers
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
