using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class DeleteFolderViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteFolderViewModel() { }
        public DeleteFolderViewModel(DeveloperUser user) : base(user)
        {

        }

        public void Recover(DeveloperUser user, string appName)
        {
            AppName = appName;
            RootRecover(user);
        }

        public bool ModelStateValid { get; set; } = true;

        [Required]
        public string AppId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SiteName { get; set; }

        [Required]
        public string Path { get; set; }
        public object AppName { get; set; }
    }
}
