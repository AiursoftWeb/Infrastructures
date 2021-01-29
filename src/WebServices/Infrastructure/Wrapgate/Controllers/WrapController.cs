using Aiursoft.Handler.Attributes;
using Aiursoft.Wrapgate.Repositories;
using Aiursoft.Wrapgate.SDK.Models;
using Aiursoft.Wrapgate.SDK.Models.AddressModels;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Wrapgate.Controllers
{
    [LimitPerMin]
    [APIExpHandler]
    [APIModelStateChecker]
    public class WrapController : Controller
    {
        private readonly HttpClient _client;
        private readonly RecordRepo _recordRepo;

        public WrapController(
            RecordRepo recordRepo)
        {
            _client = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            });
            _recordRepo = recordRepo;
        }

        [Route(template: "Wrap/{RecordName}/{**Path}", Name = "Wrap")]
        public async Task<IActionResult> Wrap(WrapAddressModel model)
        {
            var record = await _recordRepo.GetRecordByName(model.RecordName);
            if (record == null)
            {
                return NotFound();
            }
            if (!record.Enabled)
            {
                return NotFound();
            }
            var builtUrl = BuildTargetUrl(record, model.Path);
            return record.Type switch
            {
                RecordType.IFrame => View("Iframe", builtUrl),
                RecordType.Redirect => Redirect(builtUrl),
                RecordType.PermanentRedirect => RedirectPermanent(builtUrl),
                RecordType.ReverseProxy => await RewriteToUrl(builtUrl),
                _ => Redirect(builtUrl),
            };
        }

        private string BuildTargetUrl(WrapRecord record, string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return record.TargetUrl.TrimEnd('/') + "/" + path + Request.QueryString.ToString();
            }
            else
            {
                return record.TargetUrl.TrimEnd('/');
            }
        }

        private async Task<IActionResult> RewriteToUrl(string url)
        {
            var request = HttpContext.CreateProxyHttpRequest(new Uri(url));
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
            await HttpContext.CopyProxyHttpResponse(response);
            return StatusCode((int)response.StatusCode);
        }
    }
}
