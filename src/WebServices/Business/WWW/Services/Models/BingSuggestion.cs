using Newtonsoft.Json;

namespace Aiursoft.WWW.Services.Models
{
    public class QueryContext
    {
        public string OriginalQuery { get; set; }
    }

    public class SearchSuggestionsItem
    {
        public string Url { get; set; }
        public string DisplayText { get; set; }
        public string Query { get; set; }
        public string SearchKind { get; set; }
    }

    public class SuggestionGroupsItem
    {
        public string Name { get; set; }
        public SearchSuggestionsItem[] SearchSuggestions { get; set; }
    }

    public class BingSuggestion
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        public QueryContext QueryContext { get; set; }
        public SuggestionGroupsItem[] SuggestionGroups { get; set; }
    }
}
