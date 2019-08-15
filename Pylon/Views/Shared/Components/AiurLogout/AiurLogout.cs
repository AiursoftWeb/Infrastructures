using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Views.Shared.Components.AiurLogout
{
    public class AiurLogout : ViewComponent
    {
        public AiurLogout()
        {
        }

        public IViewComponentResult Invoke()
        {
            var model = new AiurLogoutViewModel
            {

            };
            return View(model);
        }
    }
}
