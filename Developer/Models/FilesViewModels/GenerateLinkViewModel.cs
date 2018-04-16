using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.FilesViewModels
{
    public class GenerateLinkViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public GenerateLinkViewModel() { }
        public GenerateLinkViewModel(DeveloperUser User) : base(User, 3) { }
        public string Address { get; set; }
        public int BucketId { get; set; }
    }
}
