using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.Probe;
using System;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class ViewFilesViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewFilesViewModel() { }
        public ViewFilesViewModel(DeveloperUser user) : base(user, 2) { }

        public Folder Folder { get; set; }
        public string AppId { get; set; }
        public string SiteName { get; set; }
        public string Path { get; set; }
        public string AppName { get; set; }
    }
}
