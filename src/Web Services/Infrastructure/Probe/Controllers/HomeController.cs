using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Probe.Controllers
{
    [LimitPerMin]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return Ok(new IndexViewModel
            {
                OpenPattern = _configuration["OpenPattern"],
                DownloadPattern = _configuration["DownloadPattern"],
                Code = ErrorType.Success,
                Message = "Welcome to Probe!"
            });
        }
    }
}
