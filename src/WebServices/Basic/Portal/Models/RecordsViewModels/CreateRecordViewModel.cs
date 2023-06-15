using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Portal.Models.AppsViewModels;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.Warpgate.SDK.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Portal.Models.RecordsViewModels;

public class CreateRecordViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public CreateRecordViewModel()
    {
    }

    public CreateRecordViewModel(PortalUser user) : base(user)
    {
    }

    [Required] [FromRoute] public string AppId { get; set; }

    [Display(Name = "Enter your new shorten URL.")]
    [Required]
    [MaxLength(50)]
    [MinLength(5)]
    [ValidDomainName]
    public string RecordName { get; set; }

    public string AppName { get; set; }

    [Display(Name = "Target URL.")]
    [Required]
    [MaxLength(1000)]
    [MinLength(5)]
    [Url]
    public string URL { get; set; }

    [Required] [Display(Name = "Type")] public RecordType Type { get; set; }

    [Required] public bool Enabled { get; set; }

    [MaxLength(1000)] public string Tags { get; set; }

    public void Recover(PortalUser user)
    {
        RootRecover(user);
    }
}