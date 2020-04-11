using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.SDK.Models.Developer;
using Aiursoft.Probe.SDK.Models;
using System;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class ViewFilesViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewFilesViewModel() { }
        public ViewFilesViewModel(DeveloperUser user) : base(user) { }

        public Folder Folder { get; set; }
        public string AppId { get; set; }
        public string SiteName { get; set; }
        public string Path { get; set; }
        public string AppName { get; set; }
    }
}
