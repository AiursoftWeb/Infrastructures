using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Views.Shared.Components.Entity
{
    public class Entity : ViewComponent
    {
        public IViewComponentResult Invoke(Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models.Thing entity)
        {
            return View(model: entity);
        }
    }
}
