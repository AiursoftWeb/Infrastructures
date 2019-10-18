using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aiursoft.Pylon.Exceptions;

namespace Aiursoft.Pylon.Services.ToGitHubServer
{
    public class GitHubService : IScopedDependency
    {
        private readonly HTTPService _http;
        private readonly HttpClient _client;

        public GitHubService(
            HTTPService http,
            IHttpClientFactory clientFactory)
        {
            _http = http;
            _client = clientFactory.CreateClient();
        }

        public async Task<string> GetAccessToken(string clientId, string clientSecret, string code)
        {
            var apiAddress = "https://github.com/login/oauth/access_token";
            var url = new AiurUrl(apiAddress, new
            {
                client_id = clientId,
                client_secret = clientSecret,
                code
            });
            var json = await _http.Get(url, false);
            var response = JsonConvert.DeserializeObject<AccessTokenResponse>(json);
            if (string.IsNullOrWhiteSpace(response.access_token))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid github crenditial");
            }
            return response.access_token;
        }

        public async Task<GitHubUserDetail> GetUserInfo(string accessToken)
        {
            var apiAddress = "https://api.github.com/user";
            var request = new HttpRequestMessage(HttpMethod.Get, apiAddress);

            request.Headers.Add("Authorization", $"token {accessToken}");
            request.Headers.Add("User-Agent", $"curl/7.65.3");

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<GitHubUserDetail>(json);
                return user;
            }
            else
            {
                throw new WebException(response.ReasonPhrase);
            }
        }
    }
}
