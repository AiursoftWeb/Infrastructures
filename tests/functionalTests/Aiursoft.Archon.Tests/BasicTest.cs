using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.Archon.SDK;
using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Archon.SDK.Services.ToArchonServer;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Archon.Tests;

[TestClass]
public class BasicTests
{
    private readonly string _endpointUrl;
    private readonly int _port;
    private HttpClient _http;
    private IHost _server;
    private ServiceProvider _serviceProvider;
    private ServiceCollection _services;

    public BasicTests()
    {
        _port = Network.GetAvailablePort();
        _endpointUrl = $"http://localhost:{_port}";
    }

    [TestInitialize]
    public async Task CreateServer()
    {
        _server = App<TestStartup>(port: _port);
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
        LimitPerMin.ClearMemory();
        if (_server != null)
        {
            await _server.StopAsync();
            _server.Dispose();
        }
    }

    [TestMethod]
    public async Task GetHome()
    {
        var response = await _http.GetAsync(_endpointUrl);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

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
            await archon.AccessTokenAsync(string.Empty, string.Empty);
            Assert.Fail("Empty request should not success.");
        }
        catch (AiurUnexpectedResponse e)
        {
            Assert.AreEqual(e.Code, ErrorType.InvalidInput);
        }
    }

    [TestMethod]
    public async Task GetTokenTest()
    {
        var archon = _serviceProvider.GetRequiredService<ArchonApiService>();
        var guid = Guid.NewGuid().ToString();
        var token = await archon.AccessTokenAsync(guid, guid);
        Assert.IsTrue(!string.IsNullOrWhiteSpace(token.AccessToken));

        var validator = _serviceProvider.GetRequiredService<ACTokenValidator>();
        var appId = validator.ValidateAccessToken(token.AccessToken);
        Assert.AreEqual(appId, guid);
    }
}