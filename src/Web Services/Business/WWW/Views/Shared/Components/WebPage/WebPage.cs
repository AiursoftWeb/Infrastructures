using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Identity.Views.Shared.Components.AiurFooter
{
    public class WebPage : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.WebPage page)
        {
            return View(model: page);
        }
    }
}
