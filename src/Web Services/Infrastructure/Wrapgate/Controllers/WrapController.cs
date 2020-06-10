using Aiursoft.Handler.Attributes;
using Aiursoft.Wrapgate.Data;
using Aiursoft.Wrapgate.Repositories;
using Aiursoft.Wrapgate.SDK.Models;
using Aiursoft.Wrapgate.SDK.Models.AddressModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrapgate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class WrapController : Controller
    {
        private readonly RecordRepo _recordRepo;

        public WrapController(RecordRepo recordRepo)
        {
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
                    return View("Iframe", record.TargetUrl);
                case RecordType.Redirect:
                    return Redirect(record.TargetUrl);
                case RecordType.PermanentRedirect:
                    return RedirectPermanent(record.TargetUrl);
                case RecordType.ReverseProxy:
                    var result = await Task.FromResult(record.TargetUrl.TrimEnd('/') + "/" + model.Path);
                    return Content(result);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
