using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Warpgate.SDK.Models.ViewModels;
using Aiursoft.Warpgate.SDK.Services;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Warpgate.Controllers;

[LimitPerMin]
public class HomeController : ControllerBase
{
    private readonly WarpgateLocator _locator;

    public HomeController(WarpgateLocator locator)
    {
        _locator = locator;
    }

    [Produces(typeof(IndexViewModel))]
    public IActionResult Index()
    {
        var model = new IndexViewModel
        {
            Code = ErrorType.Success,
            Message = "Welcome to Aiursoft Warpgate!",
            WarpPattern = _locator.WarpPattern
        };
        return this.Protocol(model);
    }
}