using Aiursoft.Developer.Models.ToolsViewModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.WebTools.Services;
using Aiursoft.XelNaga.Tools;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace Aiursoft.Developer.Controllers
{
    [LimitPerMin]
    public class ToolsController : Controller
    {
        private readonly string[] _validUuidFormats = new string[] { "N", "D", "B", "P", "X" };
        private readonly QRCodeService _qrCodeService;

        public ToolsController(QRCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

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
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public IActionResult Markdown(MarkdownViewModel model)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            model.RenderedHTML = Markdig.Markdown.ToHtml(model.SourceMarkdown ?? string.Empty, pipeline);
            return View(model);
        }

        public IActionResult UrlEncode()
        {
            var model = new UrlEncodeViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    model.ResultString = model.SourceString.ToUrlEncoded();
                }
            }
            catch (Exception e)
            {
                model.ResultString = $"Invalid input! Error message: \r\n{e.Message} \r\n {e.StackTrace}";
            }
            return View(model);
        }

        public IActionResult HtmlEncode()
        {
            var model = new HtmlEncodeViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HtmlEncode(HtmlEncodeViewModel model)
        {
            try
            {
                if (model.Decrypt)
                {
                    model.ResultString = WebUtility.HtmlDecode(model.SourceString);
                }
                else
                {
                    model.ResultString = WebUtility.HtmlEncode(model.SourceString);
                }
            }
            catch (Exception e)
            {
                model.ResultString = $"Invalid input! Error message: \r\n{e.Message} \r\n {e.StackTrace}";
            }
            return View(model);
        }

        public IActionResult JsonFormat()
        {
            var model = new JsonFormatViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult JsonFormat(JsonFormatViewModel model)
        {
            try
            {
                if (model.Format)
                {
                    var dybject = JsonConvert.DeserializeObject(model.SourceString);
                    model.ResultString = JsonConvert.SerializeObject(dybject, Formatting.Indented);
                }
                else
                {
                    var dybject = JsonConvert.DeserializeObject(model.SourceString);
                    model.ResultString = JsonConvert.SerializeObject(dybject, Formatting.None);
                }
            }
            catch (Exception e)
            {
                model.ResultString = $"Invalid input! Error message: \r\n{e.Message} \r\n {e.StackTrace}";
            }
            return View(model);
        }

        public IActionResult QRCode()
        {
            var model = new QRCodeViewModel();
            return View(model);
        }

        [Route("qrcode-build")]
        public IActionResult QRCodeBuild(string source)
        {
            var qrCode = _qrCodeService.ToQRCodePngBytes(source);
            return File(qrCode, "image/png");
        }

        public IActionResult Uuid()
        {
            var model = new UuidViewModel();
            return View(model);
        }

        [Route("uuid-build")]
        public IActionResult UuidBuild(string format = "D")
        {
            if (!_validUuidFormats.Contains(format))
            {
                format = "D";
            }
            var uuid = Guid.NewGuid().ToString(format);
            return View(model: uuid);
        }
    }
}
