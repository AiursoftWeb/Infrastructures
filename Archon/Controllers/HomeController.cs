using Aiursoft.Archon.SDK.Models;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.SDK.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Archon.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly IConfiguration _configuration;

        public HomeController(
            ServiceLocation serviceLocation,
            IConfiguration configuration)
        {
            _serviceLocation = serviceLocation;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var keyConfig = _configuration.GetSection("Key");
            return Json(new IndexViewModel
            {
                Code = ErrorType.Success,
                Message = "Welcome to Archon server! View our wiki at: " + _serviceLocation.Wiki,
                Exponent = keyConfig["Exponent"],
                Modulus = keyConfig["Modulus"]
            });
        }
    }
}
