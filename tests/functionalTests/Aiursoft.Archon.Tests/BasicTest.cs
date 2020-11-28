using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.SDK.Services.ToArchonServer;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Aiursoft.Archon.Tests
{
    [TestClass]
    public class BasicTests
    {
        private readonly string _endpointUrl = $"http://localhost:{_port}";
        private const int _port = 15999;
        private IHost _server;
        private HttpClient _http;
        private ServiceCollection _services;
        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public async Task CreateServer()
        {
            _server = Program.BuildHost(null, _port);
            _http = new HttpClient();
            _services = new ServiceCollection();
            _services.AddHttpClient();
            await _server.StartAsync();
            _services.AddArchonServer(_endpointUrl);
            _serviceProvider = _services.BuildServiceProvider();
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
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var contentObject = JsonConvert.DeserializeObject<IndexViewModel>(content);
            Assert.AreEqual(contentObject.Code, ErrorType.Success);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(contentObject.Exponent));
        }

        [TestMethod]
        public async Task CallEmptySDK()
        {
            try
            {
                var archon = _serviceProvider.GetRequiredService<ArchonApiService>();
                var result = await archon.AccessTokenAsync(string.Empty, string.Empty);
                Assert.Fail("Empty request should not success.");
            }
            catch (AiurUnexpectedResponse e)
            {
                Assert.AreEqual(e.Code, ErrorType.InvalidInput);
            }
        }
    }
}
