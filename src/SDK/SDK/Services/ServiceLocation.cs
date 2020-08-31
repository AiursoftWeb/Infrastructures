using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Services;
using DnsClient;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aiursoft.SDK.Services
{
    public class ServiceLocation : ISingletonDependency
    {
        private readonly HttpClient _client;
        public string Account { get; private set; }
        public string UI { get; private set; }
        public string Colossus { get; private set; }
        public string Developer { get; private set; }
        public string EE { get; private set; }
        public string Wiki { get; private set; }
        public string Wrap { get; private set; }
        public string WWW { get; private set; }
        public string Kahla { get; private set; }
        public string Status { get; private set; }
        public string Tracer { get; private set; }

        public ServiceLocation(
            IConfiguration configuration,
            IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
            var section = configuration.GetSection("Dependencies");
            Account = TrySet(section["AccountPath"], "https://account.aiursoft.com");
            UI = TrySet(section["UIPath"], "https://ui.aiursoft.com");
            Colossus = TrySet(section["ColossusPath"], "https://colossus.aiursoft.com");
            Wrap = TrySet(section["WrapPath"], "https://wrap.aiursoft.com");
            Developer = TrySet(section["DeveloperPath"], "https://developer.aiursoft.com");
            EE = TrySet(section["EEPath"], "https://ee.aiursoft.com");
            Wiki = TrySet(section["WikiPath"], "https://wiki.aiursoft.com");
            WWW = TrySet(section["WWWPath"], "https://www.aiursoft.com");
            Kahla = TrySet(section["KahlaPath"], "https://www.kahla.app");
            Status = TrySet(section["StatusPath"], "https://status.aiursoft.com");
            Tracer = TrySet(section["TracerPath"], "https://tracer.aiursoft.com");
            AsyncHelper.RunSync(async () =>
            {
                UI = await TryGetCDNDomain(UI);
            });
        }

        private string TrySet(string setting, string defaultValue)
        {
            return string.IsNullOrEmpty(setting) ? defaultValue : setting;
        }

        public async Task<string> TryGetCDNDomain(string input)
        {
            var domains = input.Split('.').ToList();
            domains.Insert(1, "cdn");

            var combined = string.Join(".", domains);
            try
            {
                var result = await QueryDNS(combined);
                if (!result.HasError && result.Answers.Any() && await TryConnect(combined))
                {
                    return combined;
                }
                return input;
            }
            catch
            {
                return input;
            }
        }

        private async Task<bool> TryConnect(string path)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, path);
                await _client.SendAsync(request);
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        private async Task<IDnsQueryResponse> QueryDNS(string host)
        {
            var httpsRegex = new Regex("^https://", RegexOptions.Compiled);
            var httpRegex = new Regex("^http://", RegexOptions.Compiled);
            host = httpsRegex.Replace(host, string.Empty);
            host = httpRegex.Replace(host, string.Empty);
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(host, QueryType.ANY);
            return result;
        }
    }
}
