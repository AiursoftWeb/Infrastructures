using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Canon;

using Aiursoft.Identity;
using Aiursoft.WebTools;
using Aiursoft.WWW.Data;
using Aiursoft.WWW.Models;
using Aiursoft.WWW.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Controllers;


public class SearchController : Controller
{
    private readonly BingTranslator _bingTranslator;
    private readonly CacheService _cache;
    private readonly WWWDbContext _dbContext;
    private readonly SearchService _searchService;

    public SearchController(
        SearchService searchService,
        WWWDbContext dbContext,
        BingTranslator bingTranslator,
        CacheService cache)
    {
        _searchService = searchService;
        _dbContext = dbContext;
        _bingTranslator = bingTranslator;
        _cache = cache;
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
        var result = await _cache.RunWithCache($"search-content-{market}-{page}-" + question,
            () => _searchService.DoSearch(question, market, page));
        ViewBag.Entities = await _cache.RunWithCache($"search-entity-{market}-" + question,
            () => _searchService.EntitySearch(question, market));
        if (!HttpContext.AllowTrack())
        {
            return View(result);
        }

        await _dbContext.SearchHistories.AddAsync(new SearchHistory
        {
            Question = question,
            TriggerUserId = User.GetUserId(),
            Page = page
        });
        await _dbContext.SaveChangesAsync();
        return View(result);
    }

    [Route("suggestion/{question}")]
    public async Task<IActionResult> Suggestion([FromRoute] string question)
    {
        var market = CultureInfo.CurrentCulture.Name;
        var suggestions = await _cache.RunWithCache($"search-suggestion-{market}-" + question,
            () => _searchService.GetSuggestion(question, market));
        var strings = suggestions
            ?.SuggestionGroups
            ?.FirstOrDefault(t => t.Name == "Web")
            ?.SearchSuggestions
            ?.Select(t => t.Query)
            .Take(10)
            .ToList();

        Response.Headers.Add("access-control-allow-origin", "*");
        return Json(strings);
    }

    [Route("open-search")]
    public IActionResult OpenSearch()
    {
        Response.ContentType = "text/xml";
        return View();
    }

    [Route("translate-raw/{question}")]
    public async Task<IActionResult> TranslateRaw([FromRoute] string question, [FromQuery] string lang)
    {
        var result = await _bingTranslator.CallTranslate(question, lang);
        return Json(new
        {
            value = result
        });
    }
}