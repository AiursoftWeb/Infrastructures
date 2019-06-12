using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.Probe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class ViewFilesViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewFilesViewModel() { }
        public ViewFilesViewModel(DeveloperUser user) : base(user, 5) { }

        public Folder Folder { get; set; }
        public string SiteName { get; set; }
        public string[] SitePath { get; set; }
    }
}
