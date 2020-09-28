using Aiursoft.Scanner.Interfaces;
using Aiursoft.WebTools.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.WebTools.Tests.Models
{
    public class DemoController : IScopedDependency
    {
        private readonly CannonService _cannonService;

        public DemoController(
            CannonService cannonService)
        {
            _cannonService = cannonService;
        }

        public IActionResult DemoAction()
        {
            _cannonService.Fire<DemoService>(d => d.DoSomethingSlow());
            return null;
        }

        public IActionResult DemoActionAsync()
        {
            _cannonService.FireAsync<DemoService>(d => d.DoSomethingSlowAsync());
            return null;
        }
    }
}
