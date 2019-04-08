using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Probe.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;

namespace Aiursoft.Probe.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.Protocol(ErrorType.Success, "Welcome to Probe!");
        }
    }
}
