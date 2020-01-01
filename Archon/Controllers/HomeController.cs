using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.SDK.Services;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Archon.Controllers
{
    [LimitPerMin]
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
