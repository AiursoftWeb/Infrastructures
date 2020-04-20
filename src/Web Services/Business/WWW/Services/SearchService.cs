using Aiursoft.Scanner.Interfaces;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Services
{
    public class SearchService : ISingletonDependency
    {
        private readonly string _searchAPIKey;
        private readonly WebSearchClient _client;
        private readonly EntitySearchClient _entiyClient;

        public SearchService(IConfiguration configuration)
        {
            _searchAPIKey = configuration["BingSearchAPIKey"];
            _client = new WebSearchClient(
                new Microsoft.Azure.CognitiveServices.Search.WebSearch.ApiKeyServiceClientCredentials(_searchAPIKey));
            _entiyClient = new EntitySearchClient(
                new Microsoft.Azure.CognitiveServices.Search.EntitySearch.ApiKeyServiceClientCredentials(_searchAPIKey));
        }

        public async Task<Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.SearchResponse> DoSearch(string question, int page = 1)
        {
            var webData = await _client.Web.SearchAsync(
                query: question,
                //responseFilter: new string[] { "Webpages" },
                count: 10,
                offset: (page - 1) * 10,
                safeSearch: "Off"
                );
            return webData;
        }

        public async Task<Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models.SearchResponse> EntitySearch(string question)
        {
            var entity = await _entiyClient.Entities.SearchAsync(question);
            return entity;
        }
    }
}
