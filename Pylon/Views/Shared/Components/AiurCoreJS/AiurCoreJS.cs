using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Views.Shared.Components.AiurCoreJS
{
    public class AiurCoreJS : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
