using System;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class AllAppsViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public AllAppsViewModel() { }
        public AllAppsViewModel(DeveloperUser user) : base(user)
        {
        }
    }
}
