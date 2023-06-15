using System;
using System.Collections.Generic;
using System.Linq;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Portal.Models.AppsViewModels;

[Obsolete]
public class AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public AppLayoutModel()
    {
    }

    public AppLayoutModel(PortalUser user)
    {
        RootRecover(user);
    }

    public bool EmailConfirmed { get; set; }
    public string NickName { get; set; }
    public int AppCount { get; set; }
    public IEnumerable<DirectoryApp> AllApps { get; set; }

    public void RootRecover(PortalUser user)
    {
        NickName = user.NickName;
        AppCount = user.MyApps.Count();
        AllApps = user.MyApps;
        EmailConfirmed = user.EmailConfirmed;
    }
}