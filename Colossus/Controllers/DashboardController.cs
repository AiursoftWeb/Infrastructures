using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Controllers
{
    [AiurForceAuth]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}
