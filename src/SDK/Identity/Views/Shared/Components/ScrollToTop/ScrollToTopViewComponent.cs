using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Identity.Views.Shared.Components.ScrollToTop
{
    public class ScrollToTopViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
