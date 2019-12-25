using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.Authentication.ToMicrosoftServer
{
    public class MicrosoftService : IAuthProvider
    {
        private readonly HTTPService _http;
        private readonly ServiceLocation _serviceLocation;
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public MicrosoftService(
            HTTPService http,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ServiceLocation serviceLocation,
            ILogger<MicrosoftService> logger)
        {
            _http = http;
            _serviceLocation = serviceLocation;
            _client = clientFactory.CreateClient();
            _clientId = configuration["Microsoft:ClientId"];
            _clientSecret = configuration["Microsoft:ClientSecret"];
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
            {
                logger.LogWarning("Did not set correct Microsoft credential! You can only access the service property but can execute OAuth process!");
            }
        }

        public string GetName() => "Microsoft";

        public string GetButtonColor() => "dark";

        public string GetButtonIcon() => "microsoft";

        // Line breaks for legibility only
        // https://docs.microsoft.com/zh-cn/graph/auth-v2-user // microsoft graph.

        //https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?
        //client_id=6731de76-14a6-49ae-97bc-6eba6914391e
        //&response_type=code
        //&redirect_uri=http%3A%2F%2Flocalhost%2Fmyapp%2F
        //&response_mode=query  // recommended
        //&scope=offline_access%20user.read%20mail.read
        //&state=12345
        public string GetBindRedirectLink()
        {
            return new AiurUrl("https://login.microsoftonline.com", "/common/oauth2/v2.0/authorize", new MicrosoftAuthAddressModel
            {
                ClientId = _clientId,
                ResponseType = "code",
                //Debug RedirectUri = new AiurUrl("http://localhost:41066", $"/third-party/bind-accoun/{GetName()}", new { }).ToString(),
                RedirectUri = new AiurUrl("http://localhost:41066", $"/third-party/bind-account/{GetName()}", new { }).ToString(),
                Scope = "offline_access user.read mail.read",
                State = "a",
            }).ToString();
        }

        public string GetSignInRedirectLink(AiurUrl state)
        {
            return new AiurUrl("https://login.microsoftonline.com", "/common/oauth2/v2.0/authorize", new MicrosoftAuthAddressModel
            {
                ClientId = _clientId,
                ResponseType = "code",
                RedirectUri = new AiurUrl("http://localhost:41066", $"/third-party/sign-in/{GetName()}", new { }).ToString(),
                Scope = "offline_access user.read mail.read",
                State = state.ToString()
            }).ToString();
        }

        public async Task<IUserDetail> GetUserDetail(string code, bool isBinding = false)
        {
            var token = await GetAccessToken(_clientId, _clientSecret, code, isBinding);
            return await GetUserInfo(token);
        }

        // Line breaks for legibility only
        // https://docs.microsoft.com/zh-cn/graph/auth-v2-user

        //POST /{tenant}/oauth2/v2.0/token HTTP/1.1
        //Host: https://login.microsoftonline.com
        //Content-Type: application/x-www-form-urlencoded
        //client_id = 6731de76-14a6-49ae-97bc-6eba6914391e
        //&scope=user.read%20mail.read  // Native: user.read mail.read
        //&code=OAAABAAAAiL9Kn2Z27UubvWFPbm0gLWQJVzCTE9UkP3pSx1aXxUjq3n8b2JRLk4OxVXr...
        //&redirect_uri=http%3A%2F%2Flocalhost%2Fmyapp%2F
        //&grant_type=authorization_code
        //&client_secret=JqQX2PNo9bpM0uEihUPzyrh    // NOTE: Only required for web apps

        // https://login.microsoftonline.com/common/oauth2/v2.0/token

        private async Task<string> GetAccessToken(string clientId, string clientSecret, string code, bool isBinding)
        {
            var apiAddress = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
            var url = new AiurUrl(apiAddress, new { });
            var action = isBinding ? "bind-account" : "sign-in";
            var form = new AiurUrl(string.Empty, new MicrosoftAccessTokenAddressModel
            {
                ClientId = clientId,
                Scope = "user.read mail.read",
                Code = code,
                RedirectUri = new AiurUrl("http://localhost:41066", $"/third-party/{action}/{GetName()}", new { }).ToString(),
                GranType = "authorization_code",
                ClientSecret = clientSecret
            });

            try
            {
                var json = await _http.Post(url, form, false);
                var response = JsonConvert.DeserializeObject<AccessTokenResponse>(json);
                if (string.IsNullOrWhiteSpace(response.AccessToken))
                {
                    throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid microsoft crenditial");
                }
                return response.AccessToken;
            }
            catch (WebException)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid microsoft crenditial");
            }
        }

        //https://docs.microsoft.com/zh-cn/graph/auth-v2-user#4-use-the-access-token-to-call-microsoft-graph

        //GET https://graph.microsoft.com/v1.0/me 
        //Authorization: Bearer eyJ0eXAiO ... 0X2tnSQLEANnSPHY0gKcgw
        //Host: graph.microsoft.com

        //  This api only can get false response. ??
        private async Task<MicrosoftUserDetail> GetUserInfo(string accessToken)
        {
            var apiAddress = "https://graph.microsoft.com/v1.0/me";
            var request = new HttpRequestMessage(HttpMethod.Get, apiAddress);

            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Headers.Add("User-Agent", $"curl/7.65.3");

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
