using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;

namespace Aiursoft.OSS.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class HomeController : Controller
    {
        private readonly ServiceLocation _serviceLocation;
        public HomeController(
            ServiceLocation serviceLocation)
        {
            _serviceLocation = serviceLocation;
        }

        public IActionResult Index()
        {
            return Json(new AiurProtocal
            {
                Code = ErrorType.Success,
                Message = $"Welcome to Aiursoft OSS system. Please View our document at: '{_serviceLocation.Wiki}'."
            });
        }

        public IActionResult Test()
        {
            return View();
        }
    }
}
