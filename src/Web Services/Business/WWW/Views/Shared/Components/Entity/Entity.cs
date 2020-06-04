using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Identity.Views.Shared.Components.AiurFooter
{
    public class Entity : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models.Thing entity)
        {
            return View(model: entity);
        }
    }
}
