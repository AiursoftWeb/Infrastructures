using Aiursoft.Developer.Models.SamplesViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [LimitPerMin]
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

        public ActionResult DisableWithForm()
        {
            var model = new DisableWithFormViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableWithFormSubmit()
        {
            await Task.Delay(4000);
            return RedirectToAction(nameof(DisableWithForm));
        }

        public IActionResult FormSample()
        {
            var model = new FormSampleViewModel();
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [FileChecker]
        [APIModelStateChecker]
        public async Task<IActionResult> Uploader()
        {
            if (HttpContext.Request.Form.Files.First().Length > 30 * 1024 * 1024)
            {
                return Unauthorized();
            }
            var file = Request.Form.Files.First();
            var model = await _storageService
                .SaveToProbe(file, _configuration["SampleSiteName"], DateTime.UtcNow.ToString("yyyy-MM-dd"), SaveFileOptions.SourceName);
            return Json(new
            {
                message = "Uploaded!",
                value = model.InternetPath
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm(FormSampleViewModel model)
        {
            return View(model);
        }

        public IActionResult UTCTime()
        {
            return View();
        }
    }
}
