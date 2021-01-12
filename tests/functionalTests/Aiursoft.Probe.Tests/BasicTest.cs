using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK;
using Aiursoft.Scanner;
using Aiursoft.SDK;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Stargate.Tests
{
    [TestClass]
    public class BasicTests
    {
        private readonly int _port;
        private readonly string _endpointUrl;
        private IHost _server;
        private HttpClient _http;
        private ServiceProvider _serviceProvider;

        public BasicTests()
        {
            _port = Network.GetAvailablePort();
            _endpointUrl = $"http://localhost:{_port}";
        }

        [TestInitialize]
        public async Task CreateServer()
        {
            _server = App<TestStartup>(port: _port).Update<ProbeDbContext>(); ;
            _http = new HttpClient();
            await _server.StartAsync();

            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddLibraryDependencies();
            services.AddProbeServer(_endpointUrl);
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
    }
}
