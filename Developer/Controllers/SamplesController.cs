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
    public class SamplesController : Controller
    {
        private readonly StorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly HTTPService _http;
        public SamplesController(
            StorageService storageService,
            IConfiguration configuration,
            HTTPService http)
        {
            _storageService = storageService;
            _configuration = configuration;
            _http = http;
        }

        public async Task<IActionResult> DisableWithForm()
        {
            var markdown = await _http.Get(new AiurUrl("https://raw.githubusercontent.com/Anduin2017/jquery-disable-with/master/README.md"), false);
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            var model = new DisableWithFormViewModel
            {
                DocumentHTML = html
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DisableWithFormSubmit()
        {
            await Task.Delay(4000);
            return RedirectToAction(nameof(DisableWithForm));
        }

        public async Task<IActionResult> FormSample()
        {
            var markdown = await _http.Get(new AiurUrl("https://raw.githubusercontent.com/Anduin2017/jquery-progress-upload/master/README.md"), false);
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            var model = new FormSampleViewModel
            {
                DocumentHTML = html
            };
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [FileChecker(MaxSize = 30 * 1024 * 1024)]
        [APIModelStateChecker]
        public async Task<IActionResult> Uploader()
        {
            if (HttpContext.Request.Form.Files.First().Length > 30 * 1024 * 1024)
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

        [HttpPost]
        public IActionResult SubmitForm(FormSampleViewModel model)
        {
            return View(model);
        }
    }
}
