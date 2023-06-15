using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class CreateAppViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public CreateAppViewModel()
    {
    }

    public CreateAppViewModel(PortalUser user) : base(user)
    {
    }

    [Display(Name = "App Name")]
    [Required]
    [StringLength(25, MinimumLength = 1)]
    public string AppName { get; set; }

    [Display(Name = "App Description")] public string AppDescription { get; set; }

    [Required]
    [Display(Name = "Icon Path")]
    public string IconPath { get; set; }
}