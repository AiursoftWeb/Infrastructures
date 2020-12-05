using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Observer.Tests
{
    [TestClass]
    public class BasicTests
    {
        private readonly string _endpointUrl = $"http://localhost:{_port}";
        private const int _port = 15999;
        private IHost _server;
        private HttpClient _http;
        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public async Task CreateServer()
        {
            _server = App<TestStartup>(port: _port).Update<ObserverDbContext>(); ;
            _http = new HttpClient();
            await _server.StartAsync();

            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddObserverServer(_endpointUrl);
            _serviceProvider = services.BuildServiceProvider();
        }

        [TestCleanup]
        public async Task CleanServer()
        {
            await _server.StopAsync();
            _server.Dispose();
        }

        [TestMethod]
        public async Task GetHome()
        {
            var response = await _http.GetAsync(_endpointUrl);
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var contentObject = JsonConvert.DeserializeObject<AiurProtocol>(content);
            Assert.AreEqual(contentObject.Code, ErrorType.Success);
        }

        [TestMethod]
        public async Task CallWithInvalidAccessToken()
        {
            try
            {
                var observer = _serviceProvider.GetRequiredService<EventService>();
                await observer.ViewAsync(string.Empty);
                Assert.Fail("Empty request should not success.");
            }
            catch (AiurUnexpectedResponse e)
            {
                Assert.AreEqual(e.Code, ErrorType.Unauthorized);
            }
        }

        [TestMethod]
        public async Task ViewEmptyLogsTest()
        {
            Console.WriteLine(Assembly.GetEntryAssembly().FullName);
            var observer = _serviceProvider.GetRequiredService<EventService>();
            var logs = await observer.ViewAsync("mock-access-token");
            Assert.IsTrue(!logs.Logs.Any());
        }

        [TestMethod]
        public async Task SubmitAndCheckLogTest()
        {
            Console.WriteLine(Assembly.GetEntryAssembly().FullName);
            var observer = _serviceProvider.GetRequiredService<EventService>();
            await observer.LogExceptionAsync("mock-access-token",
                new Exception("Test"));
            await Task.Delay(200);
            var logs = await observer.ViewAsync("mock-access-token");
            Assert.AreEqual(logs.Logs.SingleOrDefault().Message, "Test");
        }

        [TestMethod]
        public async Task MultipleThreadSubmitLogsTest()
        {
            Console.WriteLine(Assembly.GetEntryAssembly().FullName);
            var observer = _serviceProvider.GetRequiredService<EventService>();
            await Task.WhenAll(
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString())),
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString())),
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString())),
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString())),
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString())),
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString())),
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString())),
                observer.LogExceptionAsync("mock-access-token", new Exception(DateTime.UtcNow.Ticks.ToString()))
            );
            await Task.Delay(200);
            var logs = await observer.ViewAsync("mock-access-token");
            Assert.AreEqual(8, logs.Logs.Count);
        }
    }
}
