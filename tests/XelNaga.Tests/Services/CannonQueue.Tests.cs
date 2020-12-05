using Aiursoft.Scanner;
using Aiursoft.XelNaga.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Services
{
    [TestClass]
    public class CannonQueueTests
    {
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void Init()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddLibraryDependencies()
                .BuildServiceProvider();
        }

        [TestCleanup]
        public void Clean()
        {
            DemoService.DoneTimes = 0;
            DemoService.Done = false;
            DemoService.DoneAsync = false;
        }

        [TestMethod]
        public async Task TestCannonQueueMultipleTimes()
        {
            await TestCannonQueue();
            await Task.Delay(100);
            Clean();
            await TestCannonQueue();
        }

        [TestMethod]
        public async Task TestCannonQueue()
        {
            var controller = _serviceProvider.GetRequiredService<DemoController>();
            var startTime = DateTime.UtcNow;
            controller.QueueActionAsync();
            var endTime = DateTime.UtcNow;
            Assert.IsTrue(endTime - startTime < TimeSpan.FromMilliseconds(1000), "Demo action should finish very fast.");
            Assert.AreEqual(false, DemoService.DoneAsync, "When demo action finished, work is not over yet.");

            startTime = DateTime.UtcNow;
            while (DemoService.DoneTimes != 32)
            {
                await Task.Delay(20);
                Console.WriteLine($"Waitted for {DateTime.UtcNow - startTime}. And {DemoService.DoneTimes} tasks are finished.");
            }
            endTime = DateTime.UtcNow;
            Assert.IsTrue(endTime - startTime < TimeSpan.FromMilliseconds(1500), "All actions should finish in 1.5 second.");
        }
    }
}
