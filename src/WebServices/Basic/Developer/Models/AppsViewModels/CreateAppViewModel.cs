using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Developer.SDK.Models;

namespace Aiursoft.Developer.Models.AppsViewModels;

public class CreateAppViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public CreateAppViewModel()
    {
    }

    public CreateAppViewModel(DeveloperUser user) : base(user)
    {
    }

    [Display(Name = "App Name")]
    [Required]
    [StringLength(25, MinimumLength = 1)]
    public string AppName { get; set; }

    [Display(Name = "App Description")] public string AppDescription { get; set; }

    [Required]
    [Display(Name = "App Category")]
    public Category AppCategory { get; set; }

    [Required]
    [Display(Name = "App Platform")]
    public Platform AppPlatform { get; set; }

    [Required]
    [Display(Name = "Icon Path")]
    public string IconPath { get; set; }
}