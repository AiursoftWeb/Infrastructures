using Aiursoft.Developer.Models.AppsViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class RenameFileViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public RenameFileViewModel() { }
        public RenameFileViewModel(DeveloperUser user) : base(user)
        {

        }

        public void Recover(DeveloperUser user, string appName)
        {
            AppName = appName;
            RootRecover(user);
        }

        [Required]
        public string AppId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SiteName { get; set; }

        [Required]
        public string Path { get; set; }
        public string AppName { get; set; }

        [Required]
        public string NewName { get; set; }
    }
}
