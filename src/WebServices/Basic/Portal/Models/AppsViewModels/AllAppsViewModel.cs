using System;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class AllAppsViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public AllAppsViewModel()
    {
    }

    public AllAppsViewModel(PortalUser user) : base(user)
    {
    }
}