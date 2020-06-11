using Aiursoft.Handler.Attributes;
using Aiursoft.WebTools;
using Aiursoft.Wrapgate.Data;
using Aiursoft.Wrapgate.Repositories;
using Aiursoft.Wrapgate.SDK.Models;
using Aiursoft.Wrapgate.SDK.Models.AddressModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Wrapgate.Controllers
{
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
                AllowAutoRedirect = false
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
            var builtUrl = BuildTargetUrl(record, model.Path);
            switch (record.Type)
            {
                case RecordType.IFrame:
                    return View("Iframe", builtUrl);
                case RecordType.Redirect:
                    return Redirect(builtUrl);
                case RecordType.PermanentRedirect:
                    return RedirectPermanent(builtUrl);
                case RecordType.ReverseProxy:
                    return await RewriteToUrl(builtUrl);
                default:
                    throw new NotImplementedException();
            }
        }

        private string BuildTargetUrl(WrapRecord record, string path)
        {
            return record.TargetUrl.TrimEnd('/') + "/" + path + Request.QueryString.ToString();
        }

        private async Task<IActionResult> RewriteToUrl(string url)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = new HttpMethod(Request.Method)
            };
            foreach (var header in Request.Headers)
            {
                request.Headers.Add(header.Key, header.Value.ToString());
            }
            var response = await _client.SendAsync(request);
            foreach (var header in response.Headers)
            {
                Response.Headers.Add(header.Key, header.Value.ToString());
            }
            Response.StatusCode = (int)response.StatusCode;
            await response.Content.CopyToAsync(Response.Body);
            return Ok();
        }
    }
}
