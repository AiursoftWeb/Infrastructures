using Aiursoft.Handler.Abstract.Models;
using Aiursoft.Handler.Attributes;
using Aiursoft.Pylon;
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
