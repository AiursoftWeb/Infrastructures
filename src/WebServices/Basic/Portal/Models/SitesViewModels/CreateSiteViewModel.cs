using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Portal.Models.AppsViewModels;
using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Portal.Models.SitesViewModels;

public class CreateSiteViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public CreateSiteViewModel()
    {
    }

    public CreateSiteViewModel(PortalUser user) : base(user)
    {
    }

    [Required] [FromRoute] [IsGuidOrEmpty] public string AppId { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(5)]
    [ValidDomainName]
    public string SiteName { get; set; }

    public string AppName { get; set; }

    [Required]
    [Display(Name = "Open to upload")]
    public bool OpenToUpload { get; set; }

    [Required]
    [Display(Name = "Open to download")]
    public bool OpenToDownload { get; set; }

    public void Recover(PortalUser user)
    {
        RootRecover(user);
    }
}