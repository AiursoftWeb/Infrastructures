using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.SDK.Services;
using Aiursoft.Warpgate.Repositories;
using Aiursoft.Warpgate.SDK.Models;
using Aiursoft.Warpgate.SDK.Models.AddressModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Warpgate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class WarpController : Controller
{
    private readonly HttpClient _client;
    private readonly ILogger<WarpController> _logger;
    private readonly RecordRepo _recordRepo;

    public WarpController(
        ILogger<WarpController> logger,
        RecordRepo recordRepo)
    {
        _client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = false
        });
        _recordRepo = recordRepo;
        _logger = logger;
    }

    [Route("Warp/{RecordName}/{**Path}", Name = "Warp")]
    public async Task<IActionResult> Warp(WarpAddressModel model)
    {
        _logger.LogInformation("New request coming with name: {Record} path: {Path}", model.RecordName, model.Path);
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
        _logger.LogInformation("Target {Type} url is: {Url}", record.Type, builtUrl);
        return record.Type switch
        {
            RecordType.IFrame => View("Iframe", builtUrl),
            RecordType.Redirect => Redirect(builtUrl),
            RecordType.PermanentRedirect => RedirectPermanent(builtUrl),
            RecordType.ReverseProxy => await RewriteToUrl(builtUrl),
            _ => Redirect(builtUrl)
        };
    }

    private string BuildTargetUrl(WarpRecord record, string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            return record.TargetUrl.TrimEnd('/') + "/" + path + Request.QueryString;
        }

        return record.TargetUrl.TrimEnd('/');
    }

    private async Task<IActionResult> RewriteToUrl(string url)
    {
        if (!string.Equals(HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new AiurResponse
            {
                Message = "We can only proxy HTTP GET requests!",
                Code = Code.InvalidInput
            });
        }

        var request = HttpContext.CreateProxyHttpRequest(new Uri(url));
        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
            HttpContext.RequestAborted);
        await HttpContext.CopyProxyHttpResponse(response);
        return StatusCode((int)response.StatusCode);
    }
}