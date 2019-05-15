using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class CreateSiteViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public CreateSiteViewModel() { }
        public CreateSiteViewModel(Controller c, DeveloperUser user) : base(user, 5)
        {
            c.ViewData["PartId"] = new SelectList(this.AllApps, nameof(App.AppId), nameof(App.AppName));
        }

        public void Recover(Controller c, DeveloperUser user)
        {
            Recover(user, 5);
            c.ViewData["PartId"] = new SelectList(this.AllApps, nameof(App.AppId), nameof(App.AppName));
        }
    }
}
