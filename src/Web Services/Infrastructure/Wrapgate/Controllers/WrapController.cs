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
            switch (record.Type)
            {
                case RecordType.IFrame:
                    return View("Iframe", record.TargetUrl.TrimEnd('/') + "/" + model.Path);
                case RecordType.Redirect:
                    return Redirect(record.TargetUrl.TrimEnd('/') + "/" + model.Path);
                case RecordType.PermanentRedirect:
                    return RedirectPermanent(record.TargetUrl.TrimEnd('/') + "/" + model.Path);
                case RecordType.ReverseProxy:
                    var response = await _client.SendAsync(new HttpRequestMessage
                    {
                        RequestUri = new Uri(record.TargetUrl.TrimEnd('/') + "/" + model.Path),
                        Method = new HttpMethod(Request.Method)
                    });
                    var content = await response.Content.ReadAsStringAsync();
                    Response.StatusCode = (int)response.StatusCode;
                    Response.ContentType = response.Content.Headers.ContentType.ToString();
                    Response.ContentLength = response.Content.Headers.ContentLength;
                    await Response.WriteAsync(content);
                    return Ok();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
