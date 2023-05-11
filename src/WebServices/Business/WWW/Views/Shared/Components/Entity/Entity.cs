using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models;

namespace Aiursoft.WWW.Views.Shared.Components.Entity;

public class Entity : ViewComponent
{
    public IViewComponentResult Invoke(Thing entity)
    {
        return View(entity);
    }
}