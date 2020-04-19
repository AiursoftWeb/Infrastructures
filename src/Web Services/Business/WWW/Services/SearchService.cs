using Aiursoft.Scanner.Interfaces;
using Microsoft.Azure.CognitiveServices.Language.SpellCheck;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Services
{
    public class SearchService : ISingletonDependency
    {
        private readonly string _searchAPIKey;
        private readonly WebSearchClient _client;
        private readonly EntitySearchClient _entiyClient;
        private readonly SpellCheckClient _spellCheckClient;

        public SearchService(IConfiguration configuration)
        {
            _searchAPIKey = configuration["BingSearchAPIKey"];
            _client = new WebSearchClient(
                new Microsoft.Azure.CognitiveServices.Search.WebSearch.ApiKeyServiceClientCredentials(_searchAPIKey));
            _entiyClient = new EntitySearchClient(
                new Microsoft.Azure.CognitiveServices.Search.EntitySearch.ApiKeyServiceClientCredentials(_searchAPIKey));
            _spellCheckClient = new SpellCheckClient(
                new Microsoft.Azure.CognitiveServices.Language.SpellCheck.ApiKeyServiceClientCredentials(_searchAPIKey));
        }

        public async Task<SearchResponse> DoSearch(string question, int page = 1)
        {
            var webData = await _client.Web.SearchAsync(
                query: question,
                //responseFilter: new string[] { "Webpages" },
                count: 10,
                offset: (page - 1) * 10,
                safeSearch: "Off"
                );
            var entity = await _entiyClient.Entities.SearchAsync(question);
            return webData;
        }

        public async Task<HttpOperationResponse> SpellCheck(string question)
        {
            var data = await _spellCheckClient.SpellCheckerWithHttpMessagesAsync(text: question);
            return data;
        }
    }
}
