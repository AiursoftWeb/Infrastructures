using Aiursoft.Developer.Models.ToolsViewModels;
using Aiursoft.Pylon.Services;
using Markdig;
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

        public IActionResult Markdown()
        {
            var model = new MarkdownViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Markdown(MarkdownViewModel model)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            model.RenderedHTML = Markdig.Markdown.ToHtml(model.SourceMarkdown, pipeline);
            return View(model);
        }
    }
}
