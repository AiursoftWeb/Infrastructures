using Aiursoft.Developer.Models.ToolsViewModels;
using Aiursoft.Pylon.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            try
            {
                if (model.Decrypt)
                {
                    model.ResultString = StringOperation.Base64ToString(model.SourceString);
                }
                else
                {
                    model.ResultString = StringOperation.StringToBase64(model.SourceString);
                }
            }
            catch (Exception e)
            {
                model.ResultString = $"Invalid input! Error message: \r\n{e.Message} \r\n {e.StackTrace}";
            }
            return View(model);
        }

        public IActionResult Rot13()
        {
            var model = new Rot13ViewModel();
            return View(model);
        }

        public IActionResult Rot13(Rot13ViewModel model)
        {
            try
            {
                model.ResultString = ROT13Enc(model.SourceString);
            }
            catch (Exception e)
            {
                model.ResultString = $"Invalid input! Error message: \r\n{e.Message} \r\n {e.StackTrace}";
            }
            return View(model);
        }

        private string ROT13Enc(string input)
        {
            return !string.IsNullOrEmpty(input) ? new string(input.ToCharArray().Select(s => { return (char)((s >= 97 && s <= 122) ? ((s + 13 > 122) ? s - 13 : s + 13) : (s >= 65 && s <= 90 ? (s + 13 > 90 ? s - 13 : s + 13) : s)); }).ToArray()) : input;
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

        public IActionResult UrlEncode()
        {
            var model = new UrlEncodeViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult UrlEncode(UrlEncodeViewModel model)
        {
            try
            {
                if (model.Decrypt)
                {
                    model.ResultString = WebUtility.UrlDecode(model.SourceString);
                }
                else
                {
                    model.ResultString = WebUtility.UrlEncode(model.SourceString);
                }
            }
            catch (Exception e)
            {
                model.ResultString = $"Invalid input! Error message: \r\n{e.Message} \r\n {e.StackTrace}";
            }
            return View(model);
        }
    }
}
