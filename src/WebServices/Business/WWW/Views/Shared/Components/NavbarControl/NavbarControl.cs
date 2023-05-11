using System.Collections.Generic;
using Aiursoft.WWW.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.WWW.Views.Shared.Components.NavbarControl;

public class NavbarControl : ViewComponent
{
    public IViewComponentResult Invoke(List<Navbar> configuration)
    {
        return View(configuration);
    }
}