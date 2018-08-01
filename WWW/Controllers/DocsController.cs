using System;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW
{
    public class DocsController : Controller
    {
        public IActionResult Terms()
        {
            return View();
        }
    }
}