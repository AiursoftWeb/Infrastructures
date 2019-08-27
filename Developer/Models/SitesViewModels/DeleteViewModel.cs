using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class DeleteViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteViewModel() { }
        public DeleteViewModel(DeveloperUser user) : base(user, 2)
        {

        }

        public void Recover(DeveloperUser user, string appName)
        {
            AppName = appName;
            RootRecover(user, 5);
        }

        public bool ModelStateValid { get; set; } = true;

        [FromRoute]
        public string AppId { get; set; }

        [Required]
        [FromRoute]
        [MaxLength(50)]
        public string SiteName { get; set; }
        public string AppName { get; set; }
    }
}
