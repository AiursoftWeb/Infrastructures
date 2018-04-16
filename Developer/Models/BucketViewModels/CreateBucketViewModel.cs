using Aiursoft.Developer.Models.AppsViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.Developer.Models.BucketViewModels
{
    public class CreateBucketViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public CreateBucketViewModel() { }
        public CreateBucketViewModel(DeveloperUser User) : base(User, 2)
        {
        }
        public CreateBucketViewModel(Controller c, DeveloperUser User) : base(User, 2)
        {
            c.ViewData["PartId"] = new SelectList(this.AllApps, nameof(App.AppId), nameof(App.AppName));
        }
        public void Recover(Controller c, DeveloperUser User)
        {
            this.Recover(User, 2);
            c.ViewData["PartId"] = new SelectList(this.AllApps, nameof(App.AppId), nameof(App.AppName));
        }
        [Required]
        [Display(Name = "Selete App")]
        public string AppId { get; set; }
        [Display(Name = "New bucket name")]
        [MaxLength(25)]
        [MinLength(5)]
        [Required]
        [NoSpace]
        [NoDot]
        public string NewBucketName { get; set; }
        public bool ModelStateValid { get; set; } = true;
        [Display(Name = "Open To Read")]
        public bool OpenToRead { get; set; }
        [Display(Name = "Open To Upload")]
        public bool OpenToUpload { get; set; }
    }
}
