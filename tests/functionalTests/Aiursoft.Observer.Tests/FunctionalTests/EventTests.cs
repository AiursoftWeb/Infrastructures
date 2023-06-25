using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.SDK;
using Aiursoft.SDK.Tests.Services;
using Aiursoft.CSTools.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Observer.Tests.FunctionalTests;

[TestClass]
public class EventTests
{
    private readonly string _endpointUrl;
    private readonly int _port;
    private HttpClient _http;
    private IHost _server;
    private ServiceProvider _serviceProvider;

    public EventTests()
    {
        _port = Network.GetAvailablePort();
        _endpointUrl = $"http://localhost:{_port}";
    }

    [TestInitialize]
    public async Task CreateServer()
    {
        _server = await App<TestStartup>(port: _port).UpdateDbAsync<ObserverDbContext>();
        _http = new HttpClient();
        await _server.StartAsync();

        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddAiursoftObserver(_endpointUrl);
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestCleanup]
    public async Task CleanServer()
    {
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
        var contentObject = JsonConvert.DeserializeObject<AiurResponse>(content);
        Assert.AreEqual(contentObject.Code, Code.NoActionTaken);
    }

    [TestMethod]
    public async Task CallWithInvalidAccessToken()
    {
        try
        {
            var observer = _serviceProvider.GetRequiredService<ObserverService>();
            await observer.ViewAsync(string.Empty);
            Assert.Fail("Empty request should not success.");
        }
        catch (AiurUnexpectedServerResponseException e)
        {
            Assert.AreEqual(e.Response.Code, Code.InvalidInput);
        }
    }

    [TestMethod]
    public async Task ViewEmptyLogsTest()
    {
        var observer = _serviceProvider.GetRequiredService<ObserverService>();
        var logs = await observer.ViewAsync("mock-access-token");
        Assert.IsTrue(!logs.Logs.Any());
    }

    [TestMethod]
    public async Task SubmitAndCheckLogTest()
    {
        var observer = _serviceProvider.GetRequiredService<ObserverService>();
        await observer.LogExceptionAsync("mock-access-token", new Exception("Test"));
        await Task.Delay(500);
        var logs = await observer.ViewAsync("mock-access-token");
        Assert.AreEqual(logs.Logs.SingleOrDefault()?.Message, "Test", "Submitted error message shall be queried.");
    }

    [TestMethod]
    public async Task MultipleThreadSubmitLogsTest()
    {
        var observer = _serviceProvider.GetRequiredService<ObserverService>();
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

    [TestMethod]
    public async Task DeleteAppTest()
    {
        var observer = _serviceProvider.GetRequiredService<ObserverService>();
        await observer.LogExceptionAsync("mock-access-token", new Exception("Test"));
        await observer.DeleteAppAsync("mock-access-token", MockAcTokenValidator.MockAppId);
        await Task.Delay(500);
        var logs = await observer.ViewAsync("mock-access-token");
        Assert.AreEqual(0, logs.Logs.Count, "Shouldn't get any error log after app deleted on Observer!");
        try
        {
            await observer.DeleteAppAsync("mock2-access-token", MockAcTokenValidator.MockAppId);
            Assert.Fail("Wrong access token should not success.");
        }
        catch (AiurUnexpectedServerResponseException e)
        {
            Assert.AreEqual(Code.Unauthorized, e.Response.Code);
        }
    }
}