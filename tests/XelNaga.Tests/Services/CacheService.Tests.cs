using Aiursoft.Scanner;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Services
{
    [TestClass]
    public class CacheServiceTests
    {
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void Init()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddLibraryDependencies()
                .AddMemoryCache()
                .BuildServiceProvider();
        }

        [TestMethod]
        public async Task TestCacheAsync()
        {
            var aiurCache = _serviceProvider.GetRequiredService<AiurCache>();
            var demoService = _serviceProvider.GetRequiredService<DemoIOService>();

            {
                var startTime = DateTime.UtcNow;
                var count = await aiurCache.GetAndCache("TestCache", demoService.GetSomeCountSlowAsync);
                var endTime = DateTime.UtcNow;
                Assert.AreEqual(count, 5);
                Assert.IsTrue(endTime - startTime > TimeSpan.FromMilliseconds(190), "Demo action should finish very slow.");
            }

            {
                var startTime = DateTime.UtcNow;
                var count = await aiurCache.GetAndCache("TestCache", demoService.GetSomeCountSlowAsync);
                var endTime = DateTime.UtcNow;
                Assert.AreEqual(count, 5);
                Assert.IsTrue(endTime - startTime < TimeSpan.FromMilliseconds(10), "Demo action should finish very fast.");
            }

            aiurCache.Clear("TestCache");

            {
                var startTime = DateTime.UtcNow;
                var count = await aiurCache.GetAndCache("TestCache", demoService.GetSomeCountSlowAsync);
                var endTime = DateTime.UtcNow;
                Assert.AreEqual(count, 5);
                Assert.IsTrue(endTime - startTime > TimeSpan.FromMilliseconds(190), "Demo action should finish very slow.");
            }
        }

        [TestMethod]
        public void TestCache()
        {
            var aiurCache = _serviceProvider.GetRequiredService<AiurCache>();
            var demoService = _serviceProvider.GetRequiredService<DemoIOService>();

            {
                var startTime = DateTime.UtcNow;
                var count = aiurCache.GetAndCache("TestCache", demoService.GetSomeCountSlow);
                var endTime = DateTime.UtcNow;
                Assert.AreEqual(count, 3);
                Assert.IsTrue(endTime - startTime > TimeSpan.FromMilliseconds(190), "Demo action should finish very slow.");
            }

            {
                var startTime = DateTime.UtcNow;
                var count = aiurCache.GetAndCache("TestCache", demoService.GetSomeCountSlow);
                var endTime = DateTime.UtcNow;
                Assert.AreEqual(count, 3);
                Assert.IsTrue(endTime - startTime < TimeSpan.FromMilliseconds(10), "Demo action should finish very fast.");
            }

            aiurCache.Clear("TestCache");

            {
                var startTime = DateTime.UtcNow;
                var count = aiurCache.GetAndCache("TestCache", demoService.GetSomeCountSlow);
                var endTime = DateTime.UtcNow;
                Assert.AreEqual(count, 3);
                Assert.IsTrue(endTime - startTime > TimeSpan.FromMilliseconds(190), "Demo action should finish very slow.");
            }
        }
    }
}
