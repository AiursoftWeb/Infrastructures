using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Identity.Services.Authentication.ToMicrosoftServer
{
    public class MicrosoftService : IAuthProvider
    {
        private readonly HTTPService _http;
        private readonly GatewayLocator _serviceLocation;
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tenant;

        public MicrosoftService(
            HTTPService http,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            GatewayLocator serviceLocation,
            ILogger<MicrosoftService> logger)
        {
            _http = http;
            _serviceLocation = serviceLocation;
            _client = clientFactory.CreateClient();
            _clientId = configuration["Microsoft:ClientId"];
            _clientSecret = configuration["Microsoft:ClientSecret"];
            _tenant = string.IsNullOrWhiteSpace(configuration["Microsoft:TenantId"]) ?
                "common" : configuration["Microsoft:TenantId"];
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
            {
                logger.LogWarning("Did not set correct Microsoft credential! You can only access the service property but can execute OAuth process!");
            }
        }

        public bool IsEnabled()
        {
            return !string.IsNullOrWhiteSpace(_clientId) && !string.IsNullOrWhiteSpace(_clientSecret);
        }

        public string GetName() => "Microsoft";

        public string GetSettingsPage() => "https://account.microsoft.com/";

        public string GetButtonColor() => "warning";

        public string GetButtonIcon() => "fab fa-microsoft";

        public string GetBindRedirectLink()
        {
            return new AiurUrl("https://login.microsoftonline.com", $"/{_tenant}/oauth2/v2.0/authorize", new MicrosoftAuthAddressModel
            {
                ClientId = _clientId,
                RedirectUri = new AiurUrl(_serviceLocation.Endpoint, $"/third-party/bind-account/{GetName()}", new { }).ToString(),
                ResponseType = "code",
                Scope = "user.read",
                State = ""
            }).ToString();
        }

        public string GetSignInRedirectLink(AiurUrl state)
        {
            return new AiurUrl("https://login.microsoftonline.com", $"/{_tenant}/oauth2/v2.0/authorize", new MicrosoftAuthAddressModel
            {
                ClientId = _clientId,
                RedirectUri = new AiurUrl(_serviceLocation.Endpoint, $"/third-party/sign-in/{GetName()}", new { }).ToString(),
                ResponseType = "code",
                Scope = "user.read",
                State = state.ToString()
            }).ToString();
        }

        public async Task<IUserDetail> GetUserDetail(string code, bool isBinding = false)
        {
            var token = await GetAccessToken(_clientId, _clientSecret, code, isBinding);
            return await GetUserInfo(token);
        }

        private async Task<string> GetAccessToken(string clientId, string clientSecret, string code, bool isBinding)
        {
            var apiAddress = "https://login.microsoftonline.com" + $"/{_tenant}/oauth2/v2.0/token";
            var url = new AiurUrl(apiAddress, new { });
            var action = isBinding ? "bind-account" : "sign-in";
            var form = new AiurUrl(string.Empty, new MicrosoftAccessTokenAddressModel
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = code,
                Scope = "user.read",
                RedirectUri = new AiurUrl(_serviceLocation.Endpoint, $"/third-party/{action}/{GetName()}", new { }).ToString(),
                GrantType = "authorization_code"
            });
            try
            {
                var json = await _http.Post(url, form, false);
                var response = JsonConvert.DeserializeObject<AccessTokenResponse>(json);
                if (string.IsNullOrWhiteSpace(response.AccessToken))
                {
                    throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid Microsoft crenditial");
                }
                return response.AccessToken;
            }
            catch (WebException)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid Microsoft crenditial");
            }
        }

        private async Task<MicrosoftUserDetail> GetUserInfo(string accessToken)
        {
            var apiAddress = "https://graph.microsoft.com/v1.0/me";
            var request = new HttpRequestMessage(HttpMethod.Get, apiAddress);

            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Headers.Add("User-Agent", "curl/7.65.3");

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<MicrosoftUserDetail>(json);
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
