using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.Wrapgate.SDK.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.RecordsViewModels
{
    public class CreateRecordViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public CreateRecordViewModel() { }
        public CreateRecordViewModel(DeveloperUser user) : base(user)
        {
        }

        public void Recover(DeveloperUser user)
        {
            RootRecover(user);
        }

        public bool ModelStateValid { get; set; } = true;
        [Required]
        [FromRoute]
        public string AppId { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        public string RecordName { get; set; }

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
    }
}
