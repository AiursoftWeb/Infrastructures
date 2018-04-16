using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Developer.Models.AppsViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.BucketViewModels
{
    public class DeleteBucketViewModel : AppLayoutModel
    {

        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteBucketViewModel() { }
        public DeleteBucketViewModel(DeveloperUser User) : base(User, 2)
        {
        }
        [Display(Name = "Bucket Name")]
        public string BucketName { get; set; }
        public int FilesCount { get; set; }
        [Required]
        public string AppId { get; set; }
        [Required]
        public int BucketId { get; set; }
    }
}
