using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Identity.Views.Shared.Components.AiurFooter
{
    public class Videos : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.Videos videos)
        {
            return View(model: videos);
        }
    }
}
