using System;
using System.Collections.Generic;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public AppLayoutModel()
    {
    }

    public AppLayoutModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps)
    {
        RootRecover(user, hisApps);
    }

    public bool EmailConfirmed { get; set; }
    public string NickName { get; set; }
    public int AppCount { get; set; }
    public IEnumerable<DirectoryApp> AllApps { get; set; }

    public void RootRecover(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps)
    {
        NickName = user.NickName;
        AppCount = hisApps.Count;
        AllApps = hisApps;
        EmailConfirmed = user.EmailConfirmed;
    }
}