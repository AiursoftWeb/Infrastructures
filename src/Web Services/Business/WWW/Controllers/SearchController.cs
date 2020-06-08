using Aiursoft.Handler.Attributes;
using Aiursoft.WebTools;
using Aiursoft.WWW.Data;
using Aiursoft.WWW.Models;
using Aiursoft.WWW.Services;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Controllers
{
    [LimitPerMin(180)]
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;
        private readonly WWWDbContext _dbContext;
        private readonly BingTranslator _bingTranslator;
        private readonly AiurCache _cahce;

        public SearchController(
            SearchService searchService,
            WWWDbContext dbContext,
            BingTranslator bingTranslator,
            AiurCache cahce)
        {
            _searchService = searchService;
            _dbContext = dbContext;
            _bingTranslator = bingTranslator;
            _cahce = cahce;
        }

        [Route("search")]
        public async Task<IActionResult> DoSearch([FromQuery(Name = "q")] string question, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return Redirect("/");
            }
            ViewBag.CurrentPage = page;
            var market = CultureInfo.CurrentCulture.Name;
            var result = await _cahce.GetAndCache($"search-content-{market}-{page}-" + question, () => _searchService.DoSearch(question, market, page));
            ViewBag.Entities = await _cahce.GetAndCache($"search-entity-{market}-" + question, () => _searchService.EntitySearch(question, market));
            if (HttpContext.AllowTrack())
            {
                _dbContext.SearchHistories.Add(new SearchHistory
                {
                    Question = question,
                    TriggerUserId = User.GetUserId(),
                    Page = page
                });
                await _dbContext.SaveChangesAsync();
            }
            return View(result);
        }

        [Route("suggestion/{question}")]

        public async Task<IActionResult> Suggestion([FromRoute] string question)
        {
            var market = CultureInfo.CurrentCulture.Name;
            var suggestions = await _cahce.GetAndCache($"search-suggestion-{market}-" + question, () => _searchService.GetSuggestion(question, market));
            var strings = suggestions
                ?.SuggestionGroups
                ?.FirstOrDefault(t => t.Name == "Web")
                ?.SearchSuggestions
                ?.Select(t => t.Query)
                ?.Take(10)
                ?.ToList();
            return Json(strings);
        }

        [Route("opensearch")]
        public IActionResult OpenSearch()
        {
            Response.ContentType = "text/xml";
            return View();
        }

        [Route("TranslateRaw/{question}")]

        public IActionResult TranslateRaw([FromRoute]string question, [FromQuery]string lang)
        {
            var result = _bingTranslator.CallTranslate(question, lang);
            return Json(new
            {
                value = result
            });
        }
    }
}
