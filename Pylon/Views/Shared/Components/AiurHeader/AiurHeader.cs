using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Views.Shared.Components.AiurHeader
{
    public class AiurHeader : ViewComponent
    {
        /// <summary>
        /// Use SEO
        /// Use DisableZoom
        /// Use DNSPrefetch
        /// Use AiurFavicon
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
