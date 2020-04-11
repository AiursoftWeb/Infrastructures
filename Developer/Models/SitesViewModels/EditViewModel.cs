using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.SDK.Models.Developer;
using Aiursoft.WebTools.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class EditViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public EditViewModel() { }
        public EditViewModel(DeveloperUser user) : base(user)
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
        [MinLength(5)]
        [ValidDomainName]
        public string OldSiteName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        public string NewSiteName { get; set; }

        public string AppName { get; set; }

        [Required]
        [Display(Name = "Open to upload")]
        public bool OpenToUpload { get; set; }

        [Required]
        [Display(Name = "Open to download")]
        public bool OpenToDownload { get; set; }
        public long Size { get; set; }
    }
}
