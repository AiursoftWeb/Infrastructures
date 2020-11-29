using Aiursoft.Scanner;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Services
{
    [TestClass]
    public class HTTPServiceTests
    {
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void Init()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddHttpClient()
                .AddLibraryDependencies()
                .BuildServiceProvider();
        }

        [TestMethod]
        public void TestBuildService()
        {
            var http = _serviceProvider.GetRequiredService<HTTPService>();
            Assert.IsNotNull(http);
        }

        [TestMethod]
        public async Task TestGetInternal()
        {
            var http = _serviceProvider.GetRequiredService<HTTPService>();
            var random = StringOperation.RandomString(100);
            var result = await http.Get(new AiurUrl("https://postman-echo.com/get", new
            {
                a = random
            }), true);

            dynamic resultObject = JObject.Parse(result);
            Assert.AreEqual(resultObject.args.a.ToString(), random);
            Assert.IsTrue(resultObject.url.ToString().StartsWith("http://"));
        }

        [TestMethod]
        public async Task TestGetOutter()
        {
            var http = _serviceProvider.GetRequiredService<HTTPService>();
            var random = StringOperation.RandomString(100);
            var result = await http.Get(new AiurUrl("https://postman-echo.com/get", new
            {
                a = random
            }));

            dynamic resultObject = JObject.Parse(result);
            Assert.AreEqual(resultObject.args.a.ToString(), random);
            Assert.IsTrue(resultObject.url.ToString().StartsWith("https://"));
        }

        [TestMethod]
        public async Task TestPostInternal()
        {
            var http = _serviceProvider.GetRequiredService<HTTPService>();
            var random = StringOperation.RandomString(100);
            var random2 = StringOperation.RandomString(100);
            var result = await http.Post(new AiurUrl("https://postman-echo.com/post", new 
            {
                a = random
            }), new AiurUrl("", new
            {
                c = random2
            }), true);

            dynamic resultObject = JObject.Parse(result);
            Assert.AreEqual(resultObject.args.a.ToString(), random);
            Assert.AreEqual(resultObject.form.c.ToString(), random2);
            Assert.IsTrue(resultObject.url.ToString().StartsWith("http://"));
        }

        [TestMethod]
        public async Task TestPostOutter()
        {
            var http = _serviceProvider.GetRequiredService<HTTPService>();
            var random = StringOperation.RandomString(100);
            var random2 = StringOperation.RandomString(100);
            var result = await http.Post(new AiurUrl("https://postman-echo.com/post", new
            {
                a = random
            }), new AiurUrl("", new
            {
                c = random2
            }));

            dynamic resultObject = JObject.Parse(result);
            Assert.AreEqual(resultObject.args.a.ToString(), random);
            Assert.AreEqual(resultObject.form.c.ToString(), random2);
            Assert.IsTrue(resultObject.url.ToString().StartsWith("https://"));
        }
    }
}
