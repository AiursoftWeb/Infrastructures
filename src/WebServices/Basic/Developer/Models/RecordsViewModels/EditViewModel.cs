using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.Warpgate.SDK.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.RecordsViewModels
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

        [Required]
        [FromRoute]
        public string AppId { get; set; }

        [Required]
        [FromRoute]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        [Display(Name = "Old record name")]
        public string OldRecordName { get; set; }

        [Display(Name = "Enter your new shorten URL.")]
        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        public string NewRecordName { get; set; }

        public string AppName { get; set; }

        [Required]
        [MaxLength(1000)]
        [MinLength(5)]
        [Url]
        public string URL { get; set; }

        [Required]
        [Display(Name = "Type")]
        public RecordType Type { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [MaxLength(1000)]
        public string Tags { get; set; }
    }
}
