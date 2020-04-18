using Aiursoft.Handler.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Controllers
{
    [LimitPerMin]
    public class SearchController : Controller
    {

        [Route("search")]
        public async Task<IActionResult> DoSearch([FromQuery(Name = "q")]string question)
        {
            return Json(new { q = question });
        }
    }
}
