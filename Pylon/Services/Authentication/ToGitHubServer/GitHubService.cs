using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.Authentication.ToGitHubServer
{
    public class GitHubService : IAuthProvider
    {
        private readonly HTTPService _http;
        private readonly ServiceLocation _serviceLocation;
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public GitHubService(
            HTTPService http,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ServiceLocation serviceLocation,
            ILogger<GitHubService> logger)
        {
            _http = http;
            _serviceLocation = serviceLocation;
            _client = clientFactory.CreateClient();
            _clientId = configuration["GitHub:ClientId"];
            _clientSecret = configuration["GitHub:ClientSecret"];
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
            {
                logger.LogWarning("Did not set correct GitHub credential! You can only access the service property but can execute OAuth process!");
            }
        }

        public string GetName() => "GitHub";

        public string GetButtonColor() => "dark";

        public string GetButtonIcon() => "github";

        public string GetBindRedirectLink()
        {
            return new AiurUrl("https://github.com", "/login/oauth/authorize", new GitHubAuthAddressModel
            {
                ClientId = _clientId,
                RedirectUri = new AiurUrl(_serviceLocation.Gateway, $"/third-party/bind-account/{GetName()}", new { }).ToString(),
            }).ToString();
        }

        public string GetSignInRedirectLink(AiurUrl state)
        {
            return new AiurUrl("https://github.com", "/login/oauth/authorize", new GitHubAuthAddressModel
            {
                ClientId = _clientId,
                RedirectUri = new AiurUrl(_serviceLocation.Gateway, $"/third-party/sign-in/{GetName()}", new { }).ToString(),
                State = state.ToString()
            }).ToString();
        }

        public async Task<IUserDetail> GetUserDetail(string code, bool isBinding = false)
        {
            var token = await GetAccessToken(_clientId, _clientSecret, code);
            return await GetUserInfo(token);
        }

        private async Task<string> GetAccessToken(string clientId, string clientSecret, string code)
        {
            var apiAddress = "https://github.com/login/oauth/access_token";
            var url = new AiurUrl(apiAddress, new GitHubAccessTokenAddressModel
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = code
            });
            var json = await _http.Get(url, false);
            var response = JsonConvert.DeserializeObject<AccessTokenResponse>(json);
            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid github crenditial");
            }
            return response.AccessToken;
        }

        private async Task<GitHubUserDetail> GetUserInfo(string accessToken)
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
                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    user.Name = Guid.NewGuid().ToString();
                }
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    user.Email = user.Name + $"@from.{GetName().ToLower()}.com";
                }
                return user;
            }
            else
            {
                throw new WebException(response.ReasonPhrase);
            }
        }
    }
}
