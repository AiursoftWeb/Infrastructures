using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.SDK.Views.Shared.Components.ScrollToTop;

public class ScrollToTopViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}