using System;
using System.Collections.Generic;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class IndexViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public IndexViewModel()
    {
    }

    public IndexViewModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps) : base(user, hisApps)
    {
    }
}