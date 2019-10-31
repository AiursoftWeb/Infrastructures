using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class HTTPService : IScopedDependency
    {
        private readonly HttpClient _client;
        private readonly Regex _regex;

        public HTTPService(
            IHttpClientFactory clientFactory)
        {
            _regex = new Regex("^https://", RegexOptions.Compiled);
            _client = clientFactory.CreateClient();
        }

        public async Task<string> Get(AiurUrl url, bool internalRequest)
        {
            if (internalRequest)
            {
                url.Address = _regex.Replace(url.Address, "http://");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url.ToString())
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>())
            };

            request.Headers.Add("x-request-origin", Values.ProjectName);
            request.Headers.Add("accept", "application/json");

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
                url.Address = _regex.Replace(url.Address, "http://");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url.Address)
            {
                Content = new FormUrlEncodedContent(postDataStr.Params)
            };

            request.Headers.Add("x-request-origin", Values.ProjectName);
            request.Headers.Add("accept", "application/json");

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new WebException($"The remote server returned unexpcted status code: {response.StatusCode} - {response.ReasonPhrase}.");
            }
        }
    }
}
