using Aiursoft.Archon.SDK.Models;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Archon.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var keyConfig = _configuration.GetSection("Key");
            return Json(new IndexViewModel
            {
                Code = ErrorType.Success,
                Message = "Welcome to Archon server!",
                Exponent = keyConfig["Exponent"],
                Modulus = keyConfig["Modulus"]
            });
        }
    }
}
