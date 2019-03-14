using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;

namespace Aiursoft.Archon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ServiceLocation _serviceLocation;

        public HomeController(ServiceLocation serviceLocation)
        {
            _serviceLocation = serviceLocation;
        }

        public IActionResult Index()
        {
            return this.Protocol(ErrorType.Success, "Welcome to Archon server! View our wiki at: " + _serviceLocation.Wiki);
        }

        public IActionResult Error()
        {
            return this.Protocol(ErrorType.UnknownError, "Archon server was crashed! Please tell us!");
        }
    }
}
