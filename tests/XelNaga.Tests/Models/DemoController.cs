using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.XelNaga.Tests.Models
{
    public class DemoController : IScopedDependency
    {
        private readonly CannonService _cannonService;
        private readonly CannonQueue _cannonQueue;

        public DemoController(
            CannonService cannonService,
            CannonQueue cannonQueue)
        {
            _cannonService = cannonService;
            _cannonQueue = cannonQueue;
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

        public IActionResult QueueActionAsync()
        {
            for (int i = 0; i < 32; i++)
            {
                _cannonQueue.QueueWithDependency<DemoService>(d => d.DoSomethingSlowAsync());
            }
            return null;
        }
    }
}
