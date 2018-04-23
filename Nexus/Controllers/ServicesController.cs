using Aiursoft.Nexus.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Nexus.ServicesAddressModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Nexus.Controllers
{
    public class ServicesController : Controller
    {
        public readonly ServiceConfiguration _configuration;
        public readonly ILogger _logger;

        public ServicesController(
            ServiceConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ServicesController>();
        }

        [HttpPost]
        [APIModelStateChecker]
        public IActionResult RegisterService(RegisterServiceAddressModel model)
        {
            _configuration.Services[model.ServiceName.ToLower()] = model.ServiceAddress;
            _logger.LogInformation($"The service {model.ServiceName} was successfully registerd to {model.ServiceAddress}");
            return Json(new
            {
                code = 0,
                message = "Registered!"
            });
        }

        [APIModelStateChecker]
        public IActionResult GetService(GetServiceAddressModel model)
        {
            if (_configuration.Services.ContainsKey(model.ServiceName.ToLower()))
            {
                var address = _configuration.Services[model.ServiceName.ToLower()];
                return Json(new AiurValue<string>(address)
                {
                    Code = 0,
                    Message = "Got it!"
                });
            }
            return this.Protocal(ErrorType.NotFound, "Service name not found!");
        }
    }
}
