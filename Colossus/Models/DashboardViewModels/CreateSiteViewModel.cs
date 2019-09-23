using Aiursoft.Pylon.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class CreateSiteViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public CreateSiteViewModel()
        {
        }

        public CreateSiteViewModel(ColossusUser user) : base(user, 0, "Quick upload")
        {
        }

        public void Recover(ColossusUser user)
        {
            RootRecover(user, 0, "Quick upload");
        }

        [Display(Name = "New site name")]
        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidFolderName]
        public string SiteName { get; set; }

        [Required]
        [Display(Name = "Open to upload")]
        public bool OpenToUpload { get; set; }
    }
}
