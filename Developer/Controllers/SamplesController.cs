using Aiursoft.Developer.Models.SamplesViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    public class SamplesController : Controller
    {
        private readonly StorageService _storageService;
        private readonly IConfiguration _configuration;
        public SamplesController(
            StorageService storageService,
            IConfiguration configuration)
        {
            _storageService = storageService;
            _configuration = configuration;
        }
        public IActionResult FormSample()
        {
            var model = new FormSampleViewModel();
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [FileChecker(MaxSize = 30 * 1024 * 1024)]
        [APIModelStateChecker]
        public async Task<IActionResult> Upload()
        {
            if (HttpContext.Request.Form.Files.First().Length > 0 * 1024 * 1024)
            {
                return Unauthorized();
            }
            var file = Request.Form.Files.First();
            var model = await _storageService
                .SaveToOSSWithModel(file, Convert.ToInt32(_configuration["SampleBucket"]), 3);
            return Json(new
            {
                message = "Uploaded!",
                value = model.Path
            });
        }
    }
}
