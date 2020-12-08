using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.SDK;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK;
using Aiursoft.Stargate.SDK.Services.ToStargateServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Stargate.Tests
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
            _server = App<TestStartup>(port: _port).Update<StargateDbContext>(); ;
            _http = new HttpClient();
            await _server.StartAsync();

            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddStargateServer(_endpointUrl);
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
                var channel = _serviceProvider.GetRequiredService<ChannelService>();
                await channel.ViewMyChannelsAsync(string.Empty);
                Assert.Fail("Empty request should not success.");
            }
            catch (AiurUnexpectedResponse e)
            {
                Assert.AreEqual(e.Code, ErrorType.InvalidInput);
            }
        }

        [TestMethod]
        public async Task ViewEmptyChannelsTest()
        {
            var channel = _serviceProvider.GetRequiredService<ChannelService>();
            var channels = await channel.ViewMyChannelsAsync("mock-access-token");
            Assert.IsTrue(!channels.Channels.Any());
        }

        [TestMethod]
        public async Task CreateChannelAndViewTest()
        {
            var channel = _serviceProvider.GetRequiredService<ChannelService>();
            var created = await channel.CreateChannelAsync("mock-access-token", "Test");
            Assert.IsFalse(string.IsNullOrWhiteSpace(created.ConnectKey));
            Assert.IsFalse(string.IsNullOrWhiteSpace(created.ChannelId.ToString()));
            var channels = await channel.ViewMyChannelsAsync("mock-access-token");
            Assert.AreEqual(channels.Channels.SingleOrDefault().Description, "Test");
        }

        [TestMethod]
        public async Task MultipleThreadCreateChannelTest()
        {
            var channel = _serviceProvider.GetRequiredService<ChannelService>();
            await Task.WhenAll(
                channel.CreateChannelAsync("mock-access-token", "Test"),
                channel.CreateChannelAsync("mock-access-token", "Test"),
                channel.CreateChannelAsync("mock-access-token", "Test"),
                channel.CreateChannelAsync("mock-access-token", "Test"),
                channel.CreateChannelAsync("mock-access-token", "Test"),
                channel.CreateChannelAsync("mock-access-token", "Test"),
                channel.CreateChannelAsync("mock-access-token", "Test"),
                channel.CreateChannelAsync("mock-access-token", "Test")
            );
            await Task.Delay(200);
            var channels = await channel.ViewMyChannelsAsync("mock-access-token");
            Assert.AreEqual(8, channels.Channels.Count());
        }
    }
}
