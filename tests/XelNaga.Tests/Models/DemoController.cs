using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.XelNaga.Tests.Models
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
