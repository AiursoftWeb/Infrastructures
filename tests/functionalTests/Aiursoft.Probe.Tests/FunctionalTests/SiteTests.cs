using Aiursoft.AiurProtocol;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.Scanner;
using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Probe.Tests.FunctionalTests;

[TestClass]
public class SiteTests
{
    private readonly string _endpointUrl;
    private readonly int _port;
    private HttpClient _http;
    private IHost _server;
    private ServiceProvider _serviceProvider;

    public SiteTests()
    {
        _port = Network.GetAvailablePort();
        _endpointUrl = $"http://localhost:{_port}";
    }

    [TestInitialize]
    public async Task CreateServer()
    {
        _server = (await App<TestStartup>(port: _port).UpdateDbAsync<ProbeDbContext>(UpdateMode.RecreateThenUse));
        _http = new HttpClient();
        await _server.StartAsync();

        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddLibraryDependencies();
        services.AddAiursoftProbe(_endpointUrl);
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
        Assert.AreEqual(contentObject.Code, Code.ResultShown);
    }

    [TestMethod]
    public async Task StatusCodeTest()
    {
        var probeLocator = _serviceProvider.GetRequiredService<IOptions<ProbeConfiguration>>();
        var client = new HttpClient();
        var url = probeLocator.Value.Instance + "/sites/viewmysites";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("accept", "application/json");
        var response = await client.SendAsync(request);
        Assert.AreEqual((int)response.StatusCode, 400);

        var request2 = new HttpRequestMessage(HttpMethod.Get, url + "?accesstoken=Invalid");
        request.Headers.Add("accept", "application/json");
        var response2 = await client.SendAsync(request2);
        Assert.AreEqual((int)response2.StatusCode, 401);
    }

    [TestMethod]
    public async Task CallWithInvalidAccessToken()
    {
        try
        {
            var siteService = _serviceProvider.GetRequiredService<SitesService>();
            await siteService.ViewMySitesAsync("Invalid token");
            Assert.Fail("Empty request should not success.");
        }
        catch (AiurUnexpectedServerResponseException e)
        {
            Assert.AreEqual(Code.Unauthorized, e.Response.Code);
        }
    }

    [TestMethod]
    public async Task ViewEmptySitesTest()
    {
        var siteService = _serviceProvider.GetRequiredService<SitesService>();
        var sites = await siteService.ViewMySitesAsync("mock-access-token");
        Assert.IsTrue(!sites.Sites.Any());
    }

    [TestMethod]
    public async Task CreateSitesTest()
    {
        var siteService = _serviceProvider.GetRequiredService<SitesService>();
        var response = await siteService.CreateNewSiteAsync(
            "mock-access-token",
            "my-site",
            openToDownload: true,
            openToUpload: true);
        Assert.AreEqual(Code.JobDone, response.Code);

        var sites = await siteService.ViewMySitesAsync("mock-access-token");
        Assert.AreEqual("my-site", sites.Sites.First().SiteName);
    }

    [TestMethod]
    public async Task CreateDuplicateSitesTest()
    {
        var siteService = _serviceProvider.GetRequiredService<SitesService>();
        var response = await siteService.CreateNewSiteAsync(
            "mock-access-token",
            "my-site",
            openToDownload: true,
            openToUpload: true);
        Assert.AreEqual(Code.JobDone, response.Code);

        try
        {
            await siteService.CreateNewSiteAsync(
                "mock-access-token",
                "my-site",
                openToDownload: true,
                openToUpload: true);
            Assert.Fail("Duplicate request should not success.");
        }
        catch (AiurUnexpectedServerResponseException e)
        {
            Assert.AreEqual(Code.Conflict, e.Response.Code);
        }

        var sites = await siteService.ViewMySitesAsync("mock-access-token");
        Assert.AreEqual("my-site", sites.Sites.Single().SiteName);
    }

    [TestMethod]
    public async Task ViewSiteDetailTest()
    {
        var siteService = _serviceProvider.GetRequiredService<SitesService>();
        var response = await siteService.CreateNewSiteAsync(
            "mock-access-token",
            "my-site",
            openToDownload: false,
            openToUpload: true);
        Assert.AreEqual(Code.JobDone, response.Code);

        var sites = await siteService.ViewSiteDetailAsync("mock-access-token", "my-site");
        Assert.AreEqual(Code.ResultShown, sites.Code);
        Assert.AreEqual(sites.Site.SiteName, "my-site");
        Assert.AreEqual(sites.Site.OpenToDownload, false);
        Assert.AreEqual(sites.Site.OpenToUpload, true);
    }

    [TestMethod]
    public async Task UpdateSiteTest()
    {
        var siteService = _serviceProvider.GetRequiredService<SitesService>();
        var response = await siteService.CreateNewSiteAsync(
            "mock-access-token",
            "my-site",
            openToDownload: false,
            openToUpload: true);
        Assert.AreEqual(Code.JobDone, response.Code);

        var sites = await siteService.ViewSiteDetailAsync("mock-access-token", "my-site");
        Assert.AreEqual(sites.Site.SiteName, "my-site");
        Assert.AreEqual(sites.Site.OpenToDownload, false);
        Assert.AreEqual(sites.Site.OpenToUpload, true);

        var updateResponse = await siteService.UpdateSiteInfoAsync(
            "mock-access-token",
            "my-site",
            "my-site",
            openToDownload: true,
            openToUpload: false);
        Assert.AreEqual(Code.JobDone, updateResponse.Code);

        updateResponse = await siteService.UpdateSiteInfoAsync(
            "mock-access-token",
            "my-site",
            "my-site2",
            openToDownload: true,
            openToUpload: false);
        Assert.AreEqual(Code.JobDone, updateResponse.Code);

        sites = await siteService.ViewSiteDetailAsync("mock-access-token", "my-site2");
        Assert.AreEqual(sites.Site.SiteName, "my-site2");
        Assert.AreEqual(sites.Site.OpenToDownload, true);
        Assert.AreEqual(sites.Site.OpenToUpload, false);
    }

    [TestMethod]
    public async Task DeleteSiteTest()
    {
        var siteService = _serviceProvider.GetRequiredService<SitesService>();
        var createNewSiteResponse = await siteService.CreateNewSiteAsync(
            "mock-access-token",
            "my-site",
            openToDownload: false,
            openToUpload: true);
        Assert.AreEqual(Code.JobDone, createNewSiteResponse.Code);

        var deleteSiteResult = await siteService.DeleteSiteAsync("mock-access-token", "my-site");
        Assert.AreEqual(Code.JobDone, deleteSiteResult.Code);
        
        var sites = await siteService.ViewMySitesAsync("mock-access-token");
        Assert.AreEqual(0, sites.Sites.Count);
    }
}