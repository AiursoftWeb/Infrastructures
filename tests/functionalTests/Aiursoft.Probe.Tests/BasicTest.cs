using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.Scanner;
using Aiursoft.SDK;
using Aiursoft.XelNaga.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Probe.Tests
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
            _server = App<TestStartup>(port: _port).Update<ProbeDbContext>();
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
            LimitPerMin.ClearMemory();
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
                var siteService = _serviceProvider.GetRequiredService<SitesService>();
                await siteService.ViewMySitesAsync(string.Empty);
                Assert.Fail("Empty request should not success.");
            }
            catch (AiurUnexpectedResponse e)
            {
                Assert.AreEqual(e.Code, ErrorType.InvalidInput);
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
        public async Task TooFrequentTest()
        {
            var siteService = _serviceProvider.GetRequiredService<SitesService>();
            for (int i = 0; i < 31; i++)
            {
                await siteService.ViewMySitesAsync("mock-access-token");
            }
            try
            {
                await siteService.ViewMySitesAsync("mock-access-token");
            }
            catch (AiurUnexpectedResponse e)
            {
                Assert.AreEqual(ErrorType.TooManyRequests, e.Code);
                return;
            }
            Assert.Fail("Too many requests shall not success.");
        }

        [TestMethod]
        public async Task CreateSitesTest()
        {
            var siteService = _serviceProvider.GetRequiredService<SitesService>();
            var response = await siteService.CreateNewSiteAsync(
                accessToken: "mock-access-token",
                newSiteName: "my-site",
                openToDownload: true,
                openToUpload: true);
            Assert.AreEqual(ErrorType.Success, response.Code);

            var sites = await siteService.ViewMySitesAsync("mock-access-token");
            Assert.AreEqual("my-site", sites.Sites.FirstOrDefault().SiteName);
        }

        [TestMethod]
        public async Task CreateDuplicateSitesTest()
        {
            var siteService = _serviceProvider.GetRequiredService<SitesService>();
            var response = await siteService.CreateNewSiteAsync(
                accessToken: "mock-access-token",
                newSiteName: "my-site",
                openToDownload: true,
                openToUpload: true);
            Assert.AreEqual(ErrorType.Success, response.Code);

            try
            {
                await siteService.CreateNewSiteAsync(
                    accessToken: "mock-access-token",
                    newSiteName: "my-site",
                    openToDownload: true,
                    openToUpload: true);
                Assert.Fail("Duplicate request should not success.");
            }
            catch (AiurUnexpectedResponse e)
            {
                Assert.AreEqual(ErrorType.Conflict, e.Code);
            }

            var sites = await siteService.ViewMySitesAsync("mock-access-token");
            Assert.AreEqual("my-site", sites.Sites.SingleOrDefault().SiteName);
        }

        [TestMethod]
        public async Task ViewSiteDetailTest()
        {
            var siteService = _serviceProvider.GetRequiredService<SitesService>();
            var response = await siteService.CreateNewSiteAsync(
                accessToken: "mock-access-token",
                newSiteName: "my-site",
                openToDownload: false,
                openToUpload: true);
            Assert.AreEqual(ErrorType.Success, response.Code);

            var sites = await siteService.ViewSiteDetailAsync("mock-access-token", "my-site");
            Assert.AreEqual(sites.Site.SiteName, "my-site");
            Assert.AreEqual(sites.Site.OpenToDownload, false);
            Assert.AreEqual(sites.Site.OpenToUpload, true);
        }

        [TestMethod]
        public async Task UpdateSiteTest()
        {
            var siteService = _serviceProvider.GetRequiredService<SitesService>();
            var response = await siteService.CreateNewSiteAsync(
                accessToken: "mock-access-token",
                newSiteName: "my-site",
                openToDownload: false,
                openToUpload: true);
            Assert.AreEqual(ErrorType.Success, response.Code);

            var sites = await siteService.ViewSiteDetailAsync("mock-access-token", "my-site");
            Assert.AreEqual(sites.Site.SiteName, "my-site");
            Assert.AreEqual(sites.Site.OpenToDownload, false);
            Assert.AreEqual(sites.Site.OpenToUpload, true);

            var updateResponse = await siteService.UpdateSiteInfoAsync(
                accessToken: "mock-access-token",
                oldSiteName: "my-site",
                newSiteName: "my-site",
                openToDownload: true,
                openToUpload: false);
            Assert.AreEqual(ErrorType.Success, updateResponse.Code);

            updateResponse = await siteService.UpdateSiteInfoAsync(
                accessToken: "mock-access-token",
                oldSiteName: "my-site",
                newSiteName: "my-site2",
                openToDownload: true,
                openToUpload: false);
            Assert.AreEqual(ErrorType.Success, updateResponse.Code);

            sites = await siteService.ViewSiteDetailAsync("mock-access-token", "my-site2");
            Assert.AreEqual(sites.Site.SiteName, "my-site2");
            Assert.AreEqual(sites.Site.OpenToDownload, true);
            Assert.AreEqual(sites.Site.OpenToUpload, false);
        }
    }
}
