using Aiursoft.Handler.Attributes;
using Aiursoft.SDK.Services;
using Aiursoft.WebTools;
using Aiursoft.WWW.Data;
using Aiursoft.WWW.Models;
using Aiursoft.WWW.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Controllers
{
    [LimitPerMin]
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;
        private readonly WWWDbContext _dbContext;
        private readonly AiurCache _cahce;

        public SearchController(
            SearchService searchService,
            WWWDbContext dbContext,
            AiurCache cahce)
        {
            _searchService = searchService;
            _dbContext = dbContext;
            _cahce = cahce;
        }


        [Route("search")]
        public async Task<IActionResult> DoSearch([FromQuery(Name = "q")]string question, int page = 1)
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

        [Route("searchraw")]
        public async Task<IActionResult> SearchRaw([FromQuery(Name = "q")]string question, int page = 1)
        {
            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
            var result = await _searchService.DoSearch(question, lang, page);
            return Json(result);
        }

        [Route("opensearch")]
        public IActionResult OpenSearch()
        {
            Response.ContentType = "text/xml";
            return View();
        }
    }
}
