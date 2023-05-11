using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Views.Shared.Components.WebPage;

public class WebPage : ViewComponent
{
    public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.WebSearch.Models.WebPage page)
    {
        return View(page);
    }
}