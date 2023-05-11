using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Developer.Models.SitesViewModels;

public class EditViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public EditViewModel()
    {
    }

    public EditViewModel(DeveloperUser user) : base(user)
    {
    }

    [Required] public string AppId { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(5)]
    [ValidDomainName]
    [Display(Name = "Old site name")]
    public string OldSiteName { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(5)]
    [ValidDomainName]
    [Display(Name = "New site name")]
    public string NewSiteName { get; set; }

    public string AppName { get; set; }

    [Required]
    [Display(Name = "Open to upload")]
    public bool OpenToUpload { get; set; }

    [Required]
    [Display(Name = "Open to download")]
    public bool OpenToDownload { get; set; }

    public long Size { get; set; }

    public void Recover(DeveloperUser user, string appName)
    {
        AppName = appName;
        RootRecover(user);
    }
}