﻿using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Warpgate.Models.Configuration;
using Aiursoft.Warpgate.SDK.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aiursoft.Warpgate.Controllers;


public class HomeController : ControllerBase
{
    private readonly RedirectConfiguration _locator;

    public HomeController(IOptions<RedirectConfiguration> locator)
    {
        _locator = locator.Value;
    }

    [Produces(typeof(WarpgatePatternConfig))]
    public IActionResult Index()
    {
        var model = new WarpgatePatternConfig
        {
            Code = Code.ResultShown,
            Message = "Welcome to Aiursoft Warpgate!",
            WarpPattern = _locator.RedirectPattern
        };
        return this.Protocol(model);
    }
}