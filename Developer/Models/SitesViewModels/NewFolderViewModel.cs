using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class NewFolderViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public NewFolderViewModel() { }
        public NewFolderViewModel(DeveloperUser user) : base(user, 2) { }
        public void Recover(DeveloperUser user)
        {
            RootRecover(user, 5);
        }

        public bool ModelStateValid { get; set; } = true;
        public string NewFolderName { get; set; }

        [FromRoute]
        public string AppId { get; set; }
        [FromRoute]
        public string SiteName { get; set; }
        [FromRoute]
        public string Path { get; set; }
    }
}
