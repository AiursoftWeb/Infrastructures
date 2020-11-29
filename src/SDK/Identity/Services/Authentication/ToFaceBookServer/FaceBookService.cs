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

namespace Aiursoft.Identity.Services.Authentication.ToFaceBookServer
{
    public class FaceBookService : IAuthProvider
    {
        private readonly HTTPService _http;
        private readonly GatewayLocator _serviceLocation;
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public FaceBookService(
            HTTPService http,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            GatewayLocator serviceLocation,
            ILogger<FaceBookService> logger)
        {
            _http = http;
            _serviceLocation = serviceLocation;
            _client = clientFactory.CreateClient();
            _clientId = configuration["FaceBook:ClientId"];
            _clientSecret = configuration["FaceBook:ClientSecret"];
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
            {
                logger.LogWarning("Did not set correct FaceBook credential! You can only access the service property but can execute OAuth process!");
            }
        }

        public bool IsEnabled()
        {
            return !string.IsNullOrWhiteSpace(_clientId) && !string.IsNullOrWhiteSpace(_clientSecret);
        }

        public string GetName() => "FaceBook";

        public string GetSettingsPage() => "https://www.facebook.com/settings?tab=applications";

        public string GetButtonColor() => "secondary";

        public string GetButtonIcon() => "fab fa-facebook-square";

        public string GetBindRedirectLink()
        {
            return new AiurUrl("https://www.facebook.com", "/v5.0/dialog/oauth", new FaceBookAuthAddressModel
            {
                ClientId = _clientId,

                //Debug RedirectUri = new AiurUrl("http://localhost:41066", $"/third-party/bind-accoun/{GetName()}", new { }).ToString(),
                RedirectUri = new AiurUrl(_serviceLocation.Endpoint, $"/third-party/bind-account/{GetName()}", new { }).ToString(),
                State = "a",
                ResponseType = "code"
            }).ToString();
        }

        public string GetSignInRedirectLink(AiurUrl state)
        {
            return new AiurUrl("https://www.facebook.com", "/v5.0/dialog/oauth", new FaceBookAuthAddressModel
            {
                ClientId = _clientId,
                RedirectUri = new AiurUrl(_serviceLocation.Endpoint, $"/third-party/sign-in/{GetName()}", new { }).ToString(),
                State = state.ToString(),
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
            var apiAddress = "https://graph.facebook.com/v5.0/oauth/access_token?";
            var url = new AiurUrl(apiAddress, new { });
            var action = isBinding ? "bind-account" : "sign-in";
            var form = new AiurUrl(string.Empty, new FaceBookAccessTokenAddressModel
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = code,
                RedirectUri = new AiurUrl(_serviceLocation.Endpoint, $"/third-party/{action}/{GetName()}", new { }).ToString()
            });
            try
            {
                var json = await _http.Post(url, form);
                var response = JsonConvert.DeserializeObject<AccessTokenResponse>(json);
                if (string.IsNullOrWhiteSpace(response.AccessToken))
                {
                    throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid facebook credential");
                }
                return response.AccessToken;
            }
            catch (WebException)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Invalid facebook credential");
            }
        }


        //"https://graph.facebook.com/{your-user-id}
        //?fields=id,name
        //&access_token={your-user-access-token}"
        private async Task<FaceBookUserDetail> GetUserInfo(string accessToken)
        {
            var apiAddress = "https://graph.facebook.com/v5.0/me";
            var request = new HttpRequestMessage(HttpMethod.Get, apiAddress);

            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Headers.Add("User-Agent", "curl/7.65.3");
            request.Headers.Add("fields", "id,picture,name,email,");

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<FaceBookUserDetail>(json);
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
