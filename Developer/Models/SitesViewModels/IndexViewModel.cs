using Aiursoft.Developer.Models.AppsViewModels;
using System;

namespace Aiursoft.Developer.Models.SitesViewModels
{
    public class IndexViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public IndexViewModel() { }
        public IndexViewModel(DeveloperUser user) : base(user) { }
    }
}
