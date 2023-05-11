using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;

namespace Aiursoft.WWW.Views.Shared.Components.Related;

public class Related : ViewComponent
{
    public IViewComponentResult Invoke(RelatedSearchesRelatedSearchAnswer related)
    {
        return View(related);
    }
}