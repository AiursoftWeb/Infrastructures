using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.WWW.Services.Models;
using Aiursoft.XelNaga.Tools;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ApiKeyServiceClientCredentials =
    Microsoft.Azure.CognitiveServices.Search.WebSearch.ApiKeyServiceClientCredentials;

namespace Aiursoft.WWW.Services;

public class SearchService : ISingletonDependency
{
    private readonly WebSearchClient _client;
    private readonly EntitySearchClient _entityClient;
    private readonly HttpClient _httpClient;
    private readonly string _searchAPIKey;

    public SearchService(
        IConfiguration configuration,
        IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient();
        _searchAPIKey = configuration["BingSearchAPIKey"];
        var credential = new ApiKeyServiceClientCredentials(_searchAPIKey);
        _client = new WebSearchClient(credential);
        _entityClient = new EntitySearchClient(credential);
    }

    public async Task<SearchResponse> DoSearch(string question, string lang, int page = 1)
    {
        var webData = await _client.Web.SearchAsync(
            question,
            count: 10,
            offset: (page - 1) * 10,
            market: lang,
            setLang: lang);
        return webData;
    }

    public async Task<Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models.SearchResponse> EntitySearch(
        string question, string lang)
    {
        var entity = await _entityClient.Entities.SearchAsync(
            question,
            setLang: lang);
        return entity;
    }

    public async Task<BingSuggestion> GetSuggestion(string question, string lang)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.cognitive.microsoft.com/bing/v7.0/Suggestions" +
            $"?query={question.ToUrlEncoded()}&mkt={lang.ToUrlEncoded()}");

        request.Headers.Add("Ocp-Apim-Subscription-Key", _searchAPIKey);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BingSuggestion>(responseString);
        }

        throw new WebException(
            $"The remote server returned unexpected status code: {response.StatusCode} - {response.ReasonPhrase}.");
    }
}