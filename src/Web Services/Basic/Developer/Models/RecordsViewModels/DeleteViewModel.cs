using Aiursoft.Developer.Models.AppsViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.RecordsViewModels
{
    public class DeleteViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteViewModel() { }
        public DeleteViewModel(DeveloperUser user) : base(user)
        {

        }

        public void Recover(DeveloperUser user, string appName)
        {
            AppName = appName;
            RootRecover(user);
        }

        [FromRoute]
        [Required]
        public string AppId { get; set; }

        [Required]
        [FromRoute]
        [MaxLength(50)]
        public string RecordName { get; set; }
        public string AppName { get; set; }
    }
}
