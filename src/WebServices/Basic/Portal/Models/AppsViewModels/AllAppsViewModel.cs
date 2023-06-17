using System;
using System.Collections.Generic;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class AllAppsViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public AllAppsViewModel()
    {
    }

    public AllAppsViewModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps) : base(user, hisApps)
    {
    }
}