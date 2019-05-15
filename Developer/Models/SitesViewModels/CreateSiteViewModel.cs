using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class CreateSiteViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public CreateSiteViewModel() { }
        public CreateSiteViewModel(DeveloperUser user) : base(user, 5)
        {
            AppIds = new SelectList(AllApps, nameof(App.AppId), nameof(App.AppName));
        }

        public void Recover(DeveloperUser user)
        {
            RootRecover(user, 5);
            AppIds = new SelectList(AllApps, nameof(App.AppId), nameof(App.AppName));
        }

        public bool ModelStateValid { get; set; } = true;

        public SelectList AppIds { get; set; }

        public string AppId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SiteName { get; set; }
    }
}
