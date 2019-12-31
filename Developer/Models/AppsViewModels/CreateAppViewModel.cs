using Aiursoft.SDK.Models;
using Aiursoft.SDK.Models.Developer;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class CreateAppViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public CreateAppViewModel() { }
        public CreateAppViewModel(DeveloperUser user) : base(user) { }

        public bool ModelStateValid { get; set; } = true;

        [Display(Name = "App Name")]
        [Required]
        [StringLength(maximumLength: 25, MinimumLength = 1)]
        public string AppName { get; set; }

        [Display(Name = "App Description")]
        public string AppDescription { get; set; }

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
}
