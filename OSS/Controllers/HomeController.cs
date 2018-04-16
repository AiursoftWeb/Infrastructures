using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;

namespace Aiursoft.OSS.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Json(new AiurProtocal
            {
                Code = ErrorType.Success,
                Message = $"Welcome to Aiursoft OSS system. Please View our document at: '{Values.WikiServerAddress}'."
            });
        }

        public IActionResult Test()
        {
            return View();
        }
    }
}
