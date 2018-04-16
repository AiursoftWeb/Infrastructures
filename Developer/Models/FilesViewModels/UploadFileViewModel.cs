using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.OSS;
using Aiursoft.Developer.Models.AppsViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.FilesViewModels
{
    public class UploadFileViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public UploadFileViewModel() { }
        public UploadFileViewModel(DeveloperUser User) : base(User, 3)
        {
        }
        [Required]
        public virtual string AppId { get; set; }
        public virtual string BucketName { get; set; }
        [Required]
        public virtual int BucketId { get; set; }

        public virtual bool ModelStateValid { get; set; } = true;
        [Required]
        [Range(1, 365)]
        [Display(Name = "Alive Days")]
        public virtual int AliveDays { get; set; }
    }
}
