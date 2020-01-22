using Aiursoft.Handler.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.Protocol(ErrorType.Success, "Welcome to Probe!");
        }
    }
}
