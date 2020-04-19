using Aiursoft.Handler.Attributes;
using Aiursoft.Pylon.Services;
using Aiursoft.WWW.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Controllers
{
    [LimitPerMin]
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;
        private readonly AiurCache _cahce;

        public SearchController(
            SearchService searchService,
            AiurCache cahce)
        {
            _searchService = searchService;
            _cahce = cahce;
        }


        [Route("search")]
        public async Task<IActionResult> DoSearch([FromQuery(Name = "q")]string question, int page = 1)
        {
            ViewBag.CurrentPage = page;
            var result = await _cahce.GetAndCache($"search-content-{page}-" + question, () => _searchService.DoSearch(question, page));
            if (result.RankingResponse.Sidebar != null &&
                result.RankingResponse.Sidebar.Items != null &&
                result.RankingResponse.Sidebar.Items.Any())
            {
                ViewBag.Entities = await _cahce.GetAndCache($"search-entity-" + question, () => _searchService.EntitySearch(question));
            }
            return View(result);
        }

        [Route("searchraw")]
        public async Task<IActionResult> SearchRaw([FromQuery(Name = "q")]string question, int page = 1)
        {
            var result = await _searchService.DoSearch(question, page);
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
