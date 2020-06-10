using Aiursoft.Handler.Attributes;
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
            RecordRepo recordRepo,
            IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
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
                    var response = await _client.SendAsync(new HttpRequestMessage
                    {
                        RequestUri = new Uri(builtUrl),
                        Method = new HttpMethod(Request.Method)
                    });
                    var content = await response.Content.ReadAsStreamAsync();
                    Response.StatusCode = (int)response.StatusCode;
                    Response.ContentType = response.Content.Headers.ContentType.ToString();
                    Response.ContentLength = response.Content.Headers.ContentLength;
                    await content.CopyToAsync(Response.Body);
                    return Ok();
                default:
                    throw new NotImplementedException();
            }
        }

        private string BuildTargetUrl(WrapRecord record, string path)
        {
            return record.TargetUrl.TrimEnd('/') + "/" + path + Request.QueryString.ToString();
        }
    }
}
