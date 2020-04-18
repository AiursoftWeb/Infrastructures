using Aiursoft.Handler.Attributes;
using Aiursoft.WWW.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Controllers
{
    [LimitPerMin]
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }


        [Route("search")]
        public async Task<IActionResult> DoSearch([FromQuery(Name = "q")]string question)
        {
            var result = await _searchService.DoSearch(question);
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
