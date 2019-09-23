using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.Developer;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class CreateSiteViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public CreateSiteViewModel() { }
        public CreateSiteViewModel(DeveloperUser user) : base(user, 2)
        {
        }

        public void Recover(DeveloperUser user)
        {
            RootRecover(user, 5);
        }

        public bool ModelStateValid { get; set; } = true;
        public string AppId { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidFolderName]
        public string SiteName { get; set; }

        public string AppName { get; set; }

        [Required]
        [Display(Name = "Open to upload")]
        public bool OpenToUpload { get; set; }
    }
}
