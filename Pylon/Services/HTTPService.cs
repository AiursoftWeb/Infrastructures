using Aiursoft.Pylon.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class HTTPService
    {
        private readonly HttpClient _client;

        public HTTPService(
            IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
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

            var response = await _client.SendAsync(request);
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

            var request = new HttpRequestMessage(HttpMethod.Post, url.Address)
            {
                Content = new FormUrlEncodedContent(postDataStr.Params)
            };

            request.Headers.Add("x-request-origin", Values.ProjectName);
            var response = await _client.SendAsync(request);
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
