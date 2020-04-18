using Aiursoft.Scanner.Interfaces;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Services
{
    public class SearchService : ISingletonDependency
    {
        private readonly string _searchAPIKey;
        private readonly WebSearchClient _client;

        public SearchService(IConfiguration configuration)
        {
            _searchAPIKey = configuration["BingSearchAPIKey"];
            _client = new WebSearchClient(new ApiKeyServiceClientCredentials(_searchAPIKey));
        }

        public async Task<SearchResponse> DoSearch(string question)
        {
            var webData = await _client.Web.SearchAsync(query: question);
            return webData;
        }
    }
}
