using Aiursoft.Developer.Models.ToolsViewModels;
using Aiursoft.Pylon.Services;
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

        public IActionResult Base64()
        {
            var model = new Base64ViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Base64(Base64ViewModel model)
        {
            if(model.Decrypt)
            {
                model.ResultString = StringOperation.Base64ToString(model.SourceString);
            }
            else
            {
                model.ResultString = StringOperation.StringToBase64(model.SourceString);
            }
            return View(model);
        }
    }
}
