using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.FilesViewModels
{
    public class GenerateViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public GenerateViewModel() { }
        public GenerateViewModel(DeveloperUser user) : base(user, 3) { }
        [Required]
        public int FileId { get; set; }
        [Range(0, 1000000)]
        public int AccessTimes { get; set; } = 1;

        public string FileName { get; set; }
        public int BucketId { get; set; }
    }
}
