using Aiursoft.Handler.Attributes;
using Aiursoft.Pylon.Services;
using Aiursoft.WWW.Services;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> DoSearch([FromQuery(Name = "q")]string question)
        {
            var result = await _cahce.GetAndCache("search-content-" + question, () => _searchService.DoSearch(question));
            return View(result);
        }

        [Route("searchraw")]
        public async Task<IActionResult> SearchRaw([FromQuery(Name = "q")]string question)
        {
            var result = await _searchService.DoSearch(question);
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
