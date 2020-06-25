using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Views.Shared.Components.Related
{
    public class Related : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.RelatedSearchesRelatedSearchAnswer related)
        {
            return View(model: related);
        }
    }
}
