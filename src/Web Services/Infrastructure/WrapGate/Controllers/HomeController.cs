using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Wrapgate.SDK.Models.ViewModels;
using Aiursoft.Wrapgate.SDK.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Wrapgate.Controllers
{
    public class HomeController : Controller
    {
        private readonly WrapgateLocator _locator;

        public HomeController(WrapgateLocator locator)
        {
            _locator = locator;
        }

        [APIProduces(typeof(IndexViewModel))]
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                Code = ErrorType.Success,
                Message = "Welcome to Aiursoft Wrapgate!",
                WrapPattern = _locator.WrapPattern
            };
            return Json(model);
        }
    }
}
