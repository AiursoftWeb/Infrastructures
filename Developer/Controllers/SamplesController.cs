using Aiursoft.Developer.Models.SamplesViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    public class SamplesController : Controller
    {
        public IActionResult FormSample()
        {
            var model = new FormSampleViewModel();
            return View(model);
        }
    }
}
