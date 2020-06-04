using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Identity.Views.Shared.Components.AiurFooter
{
    public class Related : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.RelatedSearchesRelatedSearchAnswer related)
        {
            return View(model: related);
        }
    }
}
