using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Nexus.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;

namespace Aiursoft.Nexus.Controllers
{
    public class HomeController : Controller
    {
        public readonly ServiceConfiguration _configuration;
        public readonly ILogger _logger;

        public HomeController(
            ServiceConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }

        public IActionResult Index()
        {
            return this.Protocal(ErrorType.Success, "Welcome to Aiursoft Nexus server!");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
