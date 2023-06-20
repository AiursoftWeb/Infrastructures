using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;

using Aiursoft.AiurProtocol.Models;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aiursoft.Identity.Services.Authentication.ToGitHubServer;

public class GitHubService : IAuthProvider
{
    private readonly HttpClient _client;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly AiurProtocolClient  _http;
    private readonly DirectoryConfiguration _serviceLocation;

    public GitHubService(
        AiurProtocolClient  http,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        IOptions<DirectoryConfiguration> serviceLocation,
        ILogger<GitHubService> logger)
    {
        _http = http;
        _serviceLocation = serviceLocation.Value;
        _client = clientFactory.CreateClient();
        _clientId = configuration["GitHub:ClientId"];
        _clientSecret = configuration["GitHub:ClientSecret"];
        if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
        {
            logger.LogWarning(
                "Did not set correct GitHub credential! You can only access the service property but can execute OAuth process!");
        }
    }

    public bool IsEnabled()
    {
        return !string.IsNullOrWhiteSpace(_clientId) && !string.IsNullOrWhiteSpace(_clientSecret);
    }

    public string GetName()
    {
        return "GitHub";
    }

    public string GetSettingsPage()
    {
        return $"https://github.com/settings/connections/applications/{_clientId}";
    }

    public string GetButtonColor()
    {
        return "light btn-outline-secondary";
    }

    public string GetButtonIcon()
    {
        return "fab fa-github";
    }

    public string GetBindRedirectLink()
    {
        return new AiurApiEndpoint("https://github.com", "/login/oauth/authorize", new GitHubAuthAddressModel
        {
            ClientId = _clientId,
            RedirectUri = new AiurApiEndpoint(_serviceLocation.Instance, $"/third-party/bind-account/{GetName()}", new { })
                .ToString()
        }).ToString();
    }

    public string GetSignInRedirectLink(AiurUrl state)
    {
        return new AiurApiEndpoint("https://github.com", "/login/oauth/authorize", new GitHubAuthAddressModel
        {
            ClientId = _clientId,
            RedirectUri =
                new AiurApiEndpoint(_serviceLocation.Instance, $"/third-party/sign-in/{GetName()}", new { }).ToString(),
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
        var url = new AiurApiEndpoint(apiAddress, new GitHubAccessTokenAddressModel
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            Code = code
        });
        var json = await _http.Get(url);
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
        request.Headers.Add("User-Agent", "curl/7.65.3");

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

        throw new WebException(response.ReasonPhrase);
    }
}