using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Identity.Services.Authentication.ToFaceBookServer;

public class FaceBookService : IAuthProvider
{
    private readonly HttpClient _client;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly HttpService  _http;
    private readonly DirectoryConfiguration _serviceLocation;

    public FaceBookService(
        HttpService http,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        IOptions<DirectoryConfiguration> serviceLocation,
        ILogger<FaceBookService> logger)
    {
        _http = http;
        _serviceLocation = serviceLocation.Value;
        _client = clientFactory.CreateClient();
        _clientId = configuration["FaceBook:ClientId"];
        _clientSecret = configuration["FaceBook:ClientSecret"];
        if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
        {
            logger.LogWarning(
                "Did not set correct FaceBook credential! You can only access the service property but can execute OAuth process!");
        }
    }

    public bool IsEnabled()
    {
        return !string.IsNullOrWhiteSpace(_clientId) && !string.IsNullOrWhiteSpace(_clientSecret);
    }

    public string GetName()
    {
        return "FaceBook";
    }

    public string GetSettingsPage()
    {
        return "https://www.facebook.com/settings?tab=applications";
    }

    public string GetButtonColor()
    {
        return "secondary";
    }

    public string GetButtonIcon()
    {
        return "fab fa-facebook-square";
    }

    public string GetBindRedirectLink()
    {
        return new AiurApiEndpoint("https://www.facebook.com", "/v5.0/dialog/oauth", new FaceBookAuthAddressModel
        {
            ClientId = _clientId,

            //Debug RedirectUri = new AiurApiEndpoint("http://localhost:41066", $"/third-party/bind-account/{GetName()}", new { }).ToString(),
            RedirectUri = new AiurApiEndpoint(_serviceLocation.Instance, $"/third-party/bind-account/{GetName()}", new { })
                .ToString(),
            State = "a",
            ResponseType = "code"
        }).ToString();
    }

    public string GetSignInRedirectLink(string state)
    {
        return new AiurApiEndpoint("https://www.facebook.com", "/v5.0/dialog/oauth", new FaceBookAuthAddressModel
        {
            ClientId = _clientId,
            RedirectUri =
                new AiurApiEndpoint(_serviceLocation.Instance, $"/third-party/sign-in/{GetName()}", new { }).ToString(),
            State = state,
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
        var action = isBinding ? "bind-account" : "sign-in";
        var form = new ApiPayload(new FaceBookAccessTokenAddressModel
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            Code = code,
            RedirectUri =
                new AiurApiEndpoint(_serviceLocation.Instance, $"/third-party/{action}/{GetName()}", new { }).ToString()
        });
        try
        {
            var json = await _http.Post(apiAddress, form.Params);
            var response = JsonConvert.DeserializeObject<AccessTokenResponse>(json);
            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new InvalidOperationException("Invalid facebook credential");
            }

            return response.AccessToken;
        }
        catch (WebException)
        {
            throw new InvalidOperationException("Invalid facebook credential");
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

        throw new WebException(response.ReasonPhrase);
    }
}