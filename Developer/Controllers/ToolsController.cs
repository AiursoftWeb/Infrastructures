using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    public class ToolsController : Controller
    {
        public IActionResult WebSocket()
        {
            return View();
        }
    }
}
