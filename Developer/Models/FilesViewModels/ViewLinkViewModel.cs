using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.FilesViewModels
{
    public class ViewLinkViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewLinkViewModel() { }
        public ViewLinkViewModel(DeveloperUser user) : base(user, 3) { }
        public string Address { get; set; }
        public int BucketId { get; set; }
    }
}
