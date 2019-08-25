using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

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
