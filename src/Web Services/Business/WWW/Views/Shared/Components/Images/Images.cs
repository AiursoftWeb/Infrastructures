using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Identity.Views.Shared.Components.AiurFooter
{
    public class Images : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.Images images)
        {
            return View(model: images);
        }
    }
}
