using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.Authentication.ToGoogerServer
{
    public class GoogleService : IAuthProvider
    {
        private readonly HTTPService _http;
        private readonly ServiceLocation _serviceLocation;
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public GoogleService(
            HTTPService http,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ServiceLocation serviceLocation,
            ILogger<GoogleService> logger)
        {
            _http = http;
            _serviceLocation = serviceLocation;
            _client = clientFactory.CreateClient();
            _clientId = configuration["Google:ClientId"];
            _clientSecret = configuration["Google:ClientSecret"];
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
            {
                logger.LogWarning("Did not set correct Google credential! You can only access the service property but can execute OAuth process!");
            }
        }

        public string GetName() => "Google";

        public string GetButtonColor() => "primary";

        public string GetButtonIcon() => "google";

        public string GetBindRedirectLink()
        {
            return new AiurUrl("https://accounts.google.com", "/o/oauth2/v2/auth", new GoogleAuthAddressModel
            {
                ClientId = _clientId,
                RedirectUri = new AiurUrl(_serviceLocation.Gateway, $"/third-party/bind-account/{GetName()}", new { }).ToString(),
                State = "a",
                Scope = "profile",
                ResponseType = "code"
            }).ToString();
        }

        public string GetSignInRedirectLink(AiurUrl state)
        {
            return new AiurUrl("https://accounts.google.com", "/o/oauth2/v2/auth", new GoogleAuthAddressModel
            {
                ClientId = _clientId,
                RedirectUri = new AiurUrl(_serviceLocation.Gateway, $"/third-party/sign-in/{GetName()}", new { }).ToString(),
                State = state.ToString(),
                Scope = "profile",
                ResponseType = "code"
            }).ToString();
        }

        public async Task<IUserDetail> GetUserDetail(string code, bool isBinding = false)
        {
            var token = await GetAccessToken(_clientId, _clientSecret, code, isBinding);
            return await GetUserInfo(token);
        }

        private async Task<string> GetAccessToken(string clientId, string clientSecret, string code, bool isBinding)
        {
            var apiAddress = "https://oauth2.googleapis.com/token";
            var url = new AiurUrl(apiAddress, new { });
            var action = isBinding ? "bind-account" : "sign-in";
            var form = new AiurUrl(string.Empty, new GoogleAccessTokenAddressModel
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = code,
                RedirectUri = new AiurUrl(_serviceLocation.Gateway, $"/third-party/{action}/{GetName()}", new { }).ToString(),
                GrantType = "authorization_code"
            });
            try
            {
                var json = await _http.Post(url, form, false);
                var response = JsonConvert.DeserializeObject<AccessTokenResponse>(json);
                if (string.IsNullOrWhiteSpace(response.AccessToken))
                {
                    throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid google crenditial");
                }
                return response.AccessToken;
            }
            catch (WebException)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid google crenditial");
            }
        }

        private async Task<GoogleUserDetail> GetUserInfo(string accessToken)
        {
            var apiAddress = "https://www.googleapis.com/oauth2/v3/userinfo";
            var request = new HttpRequestMessage(HttpMethod.Get, apiAddress);

            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Headers.Add("User-Agent", $"curl/7.65.3");

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<GoogleUserDetail>(json);
                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    user.Name = Guid.NewGuid().ToString();
                }
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    user.Email = user.Name.Replace(' ', '_') + $"@from.{GetName().ToLower()}.com";
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
