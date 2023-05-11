using System;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Developer.Models.AppsViewModels;

public class AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public AppLayoutModel()
    {
    }

    public AppLayoutModel(DeveloperUser user)
    {
        RootRecover(user);
    }

    public bool EmailConfirmed { get; set; }
    public string NickName { get; set; }
    public int AppCount { get; set; }
    public IEnumerable<DeveloperApp> AllApps { get; set; }

    public void RootRecover(DeveloperUser user)
    {
        NickName = user.NickName;
        AppCount = user.MyApps.Count();
        AllApps = user.MyApps;
        EmailConfirmed = user.EmailConfirmed;
    }
}