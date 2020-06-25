using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Views.Shared.Components.Images
{
    public class Images : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.Images images)
        {
            return View(model: images);
        }
    }
}
