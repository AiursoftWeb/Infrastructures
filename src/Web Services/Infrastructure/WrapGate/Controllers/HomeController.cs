using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Wrapgate.SDK.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Wrapgate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [APIProduces(typeof(IndexViewModel))]
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                Code = ErrorType.Success,
                Message = "Welcome to Aiursoft Wrapgate!",
                WrapPattern = _configuration["WrapPattern"]
            };
            return Json(model);
        }
    }
}
