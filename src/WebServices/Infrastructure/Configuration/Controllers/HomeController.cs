using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Configuration.Controllers
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return this.Protocol(ErrorType.Success, "Welcome to Aiursoft Configuration center!");
        }
    }
}
