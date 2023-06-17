using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Portal.Models.AppsViewModels;

namespace Aiursoft.Portal.Models.SitesViewModels;

public class DeleteFolderViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public DeleteFolderViewModel()
    {
    }

    public DeleteFolderViewModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps) : base(user, hisApps)
    {
    }

    [Required] public string AppId { get; set; }

    [Required] [MaxLength(50)] public string SiteName { get; set; }

    [Required] public string Path { get; set; }

    public object AppName { get; set; }

    public void Recover(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps, string appName)
    {
        AppName = appName;
        RootRecover(user, hisApps);
    }
}