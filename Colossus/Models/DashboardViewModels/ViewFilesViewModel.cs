using Aiursoft.Pylon.Models.Probe;
using System;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class ViewFilesViewModel : LayoutViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewFilesViewModel() { }
        public ViewFilesViewModel(ColossusUser user) : base(user, "View files") { }

        public Folder Folder { get; set; }
        public string Path { get; set; }
        public string SiteName { get; set; }
    }
}
