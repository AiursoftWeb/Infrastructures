using Aiursoft.WWW.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Aiursoft.WWW.Views.Shared.Components.NavbarControl
{
    public class NavbarControl : ViewComponent
    {
        public IViewComponentResult Invoke(List<Navbar> configuration)
        {
            return View(model: configuration);
        }
    }
}
