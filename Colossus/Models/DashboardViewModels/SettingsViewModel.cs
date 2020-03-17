using Aiursoft.SDK.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class SettingsViewModel : LayoutViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public SettingsViewModel() { }
        public SettingsViewModel(ColossusUser user) : base(user, "Settings") { }
        public void Recover(ColossusUser user)
        {
            RootRecover(user, "Settings");
        }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        public string OldSiteName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        [Display(Name = "Name of your new site")]
        public string NewSiteName { get; set; }

        [Required]
        [Display(Name = "Open to upload")]
        public bool OpenToUpload { get; set; }

        [Required]
        [Display(Name = "Open to download")]
        public bool OpenToDownload { get; set; }

        public long SiteSize { get; set; }
    }
}
