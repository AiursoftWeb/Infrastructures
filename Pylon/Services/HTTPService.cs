using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class HTTPService
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly CookieContainer _cc;

        public HTTPService(
            ILogger<HTTPService> logger,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _cc = new CookieContainer();
        }

        public async Task<string> Get(AiurUrl url, bool internalRequest)
        {
            if (internalRequest)
            {
                url.Address = url.Address.Replace("https://", "http://");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url.Address)
            {
                Content = new FormUrlEncodedContent(url.Params)
            };

            request.Headers.Add("x-request-origin", Values.ProjectName);

            var response = await _clientFactory.CreateClient().SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new WebException(response.ReasonPhrase);
            }
        }

        public async Task<string> Post(AiurUrl url, AiurUrl postDataStr, bool internalRequest)
        {
            if (internalRequest)
            {
                url.Address = url.Address.Replace("https://", "http://");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url.Address)
            {
                Content = new FormUrlEncodedContent(postDataStr.Params)
            };

            request.Headers.Add("x-request-origin", Values.ProjectName);
            var response = await _clientFactory.CreateClient().SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new WebException(response.ReasonPhrase);
            }
        }
    }
}
