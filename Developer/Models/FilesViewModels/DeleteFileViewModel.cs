using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Developer.Models.AppsViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.FilesViewModels
{
    public class DeleteFileViewModel : AppLayoutModel
    {

        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteFileViewModel() { }
        public DeleteFileViewModel(DeveloperUser User) : base(User, 3) { }
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        public int FileId { get; set; }

        public int BucketId { get; set; }
    }
}
