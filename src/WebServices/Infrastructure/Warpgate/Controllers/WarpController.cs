using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Warpgate.Repositories;
using Aiursoft.Warpgate.SDK.Models;
using Aiursoft.Warpgate.SDK.Models.AddressModels;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Warpgate.Controllers
{
    [LimitPerMin]
    [APIExpHandler]
    [APIModelStateChecker]
    public class WarpController : Controller
    {
        private readonly HttpClient _client;
        private readonly RecordRepo _recordRepo;

        public WarpController(
            RecordRepo recordRepo)
        {
            _client = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            });
            _recordRepo = recordRepo;
        }

        [Route(template: "Warp/{RecordName}/{**Path}", Name = "Warp")]
        public async Task<IActionResult> Warp(WarpAddressModel model)
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

        private string BuildTargetUrl(WarpRecord record, string path)
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
            if (!string.Equals(HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new AiurProtocol
                {
                    Message = "We can only proxy HTTP GET requests!",
                    Code = ErrorType.InvalidInput
                });
            }

            var request = HttpContext.CreateProxyHttpRequest(new Uri(url));
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
            await HttpContext.CopyProxyHttpResponse(response);
            return StatusCode((int)response.StatusCode);
        }
    }
}
