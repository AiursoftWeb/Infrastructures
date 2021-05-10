using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner;
using Aiursoft.SDK;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.Stargate.SDK.Services;
using Aiursoft.Stargate.SDK.Services.ToStargateServer;
using Aiursoft.Stargate.Tests.Services;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
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
            _server = App<TestStartup>(port: _port).Update<StargateDbContext>();
            _http = new HttpClient();
            await _server.StartAsync();

            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddLibraryDependencies();
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
            Assert.AreEqual(channels.Channels.Single().Description, "Test");
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

        private static int MessageCount;
        private static async Task Monitor(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            while (true)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    MessageCount++;
                    var _ = Encoding.UTF8.GetString(buffer.Skip(buffer.Offset).Take(buffer.Count).ToArray())
                        .Trim('\0')
                        .Trim();
                }
                else
                {
                    Console.WriteLine($"[WebSocket Event] Remote wrong message. [{result.MessageType}].");
                    break;
                }
            }
        }

        [TestMethod]
        public async Task TestConnect()
        {
            var locator = _serviceProvider.GetRequiredService<StargateLocator>();
            var channelService = _serviceProvider.GetRequiredService<ChannelService>();
            var messageSender = _serviceProvider.GetRequiredService<DebugMessageSender>();
            var channel = await channelService.CreateChannelAsync("mock-access-token", "Connect test channel");

            var wsPath = new AiurUrl(locator.ListenEndpoint, "Listen", "Channel", new ChannelAddressModel
            {
                Id = channel.ChannelId,
                Key = channel.ConnectKey
            }).ToString();

            using (var socket = new ClientWebSocket())
            {
                await socket.ConnectAsync(new Uri(wsPath), CancellationToken.None);
                Console.WriteLine("Websocket connected!");
                await Task.Delay(50);
                await Task.Factory.StartNew(() => Monitor(socket));
                await Task.Delay(50);
                await messageSender.SendDebuggingMessages("mock-access-token", channel.ChannelId);
                await Task.Delay(50);
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

            // Wait for complete
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(200);
            }
            Assert.AreEqual(100, MessageCount);
        }
    }
}
