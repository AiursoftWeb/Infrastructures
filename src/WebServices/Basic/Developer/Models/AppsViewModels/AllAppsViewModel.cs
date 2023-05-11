using System;

namespace Aiursoft.Developer.Models.AppsViewModels;

public class AllAppsViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public AllAppsViewModel()
    {
    }

    public AllAppsViewModel(DeveloperUser user) : base(user)
    {
    }
}