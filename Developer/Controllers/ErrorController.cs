using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Developer.Controllers
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
