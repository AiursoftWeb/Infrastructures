using Aiursoft.AiurProtocol;
using Aiursoft.Scanner;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.SDK;
using Aiursoft.Warpgate.SDK.Models;
using Aiursoft.Warpgate.SDK.Services.ToWarpgateServer;
using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Warpgate.Tests;

[TestClass]
public class BasicTests
{
    private readonly string _endpointUrl;
    private readonly int _port;
    private HttpClient _http;
    private IHost _server;
    private ServiceProvider _serviceProvider;

    public BasicTests()
    {
        _port = Network.GetAvailablePort();
        _endpointUrl = $"http://localhost:{_port}";
    }

    [TestInitialize]
    public async Task CreateServer()
    {
        _server = (await App<TestStartup>(Array.Empty<string>(), port: _port).UpdateDbAsync<WarpgateDbContext>(UpdateMode.RecreateThenUse));
        _http = new HttpClient();
        await _server.StartAsync();

        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddLibraryDependencies();
        services.AddAiursoftWarpgate(_endpointUrl);
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
        Assert.AreEqual(Code.ResultShown, contentObject.Code);
    }

    [TestMethod]
    public async Task CallWithInvalidAccessToken()
    {
        try
        {
            var records = _serviceProvider.GetRequiredService<RecordsService>();
            await records.ViewMyRecordsAsync(string.Empty);
            Assert.Fail("Empty request should not success.");
        }
        catch (AiurUnexpectedServerResponseException e)
        {
            Assert.AreEqual(Code.InvalidInput, e.Response.Code);
        }
    }

    [TestMethod]
    public async Task ViewEmptyRecordsTest()
    {
        var records = _serviceProvider.GetRequiredService<RecordsService>();
        var recordsItems = await records.ViewMyRecordsAsync("mock-access-token");
        Assert.IsTrue(!recordsItems.Records.Any());
    }

    [TestMethod]
    public async Task CreateRecordAndViewTest()
    {
        var recordsService = _serviceProvider.GetRequiredService<RecordsService>();
        var created = await recordsService.CreateNewRecordAsync("mock-access-token", "stack",
            "https://stackoverflow.com", Array.Empty<string>(), RecordType.Redirect, true);
        Assert.AreEqual(Code.JobDone, created.Code);
        var records = await recordsService.ViewMyRecordsAsync("mock-access-token");
        Assert.AreEqual(records.Records.Single().RecordUniqueName, "stack");
    }

    [TestMethod]
    public async Task TagSystemTest()
    {
        var recordsService = _serviceProvider.GetRequiredService<RecordsService>();
        await recordsService.CreateNewRecordAsync("mock-access-token", "stack0", "https://stackoverflow.com",
            Array.Empty<string>(), RecordType.Redirect, true);
        await recordsService.CreateNewRecordAsync("mock-access-token", "stack1", "https://stackoverflow.com",
            new[] { "a", "b" }, RecordType.Redirect, true);
        await recordsService.CreateNewRecordAsync("mock-access-token", "stack2", "https://stackoverflow.com",
            new[] { "a", "c" }, RecordType.Redirect, true);
        var records = await recordsService.ViewMyRecordsAsync("mock-access-token", "b");
        Assert.AreEqual(records.Records.Single().RecordUniqueName, "stack1");
    }
}