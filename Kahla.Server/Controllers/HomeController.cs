using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using System.IO;
using Aiursoft.Pylon.Models;

namespace Kahla.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.Protocal(ErrorType.Success, "Welcome to kahla server! View our wiki at: " + Values.WikiServerAddress);
        }

        public IActionResult Error()
        {
            return this.Protocal(ErrorType.UnknownError, "Kahla server was crashed! Please tell us!");
        }
    }
}
