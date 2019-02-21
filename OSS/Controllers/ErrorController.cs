using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.OSS.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Code404()
        {
            return View();
        }

        public IActionResult ServerException()
        {
            return this.Protocol(ErrorType.UnknownError, "Kahla server was crashed! Please tell us!");
        }
    }
}
