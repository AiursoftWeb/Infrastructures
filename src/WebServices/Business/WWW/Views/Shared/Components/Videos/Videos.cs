using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Views.Shared.Components.Videos
{
    public class Videos : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.Videos videos)
        {
            return View(model: videos);
        }
    }
}
