using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Portal.Models.AppsViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Portal.Models.SitesViewModels;

public class DeleteViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public DeleteViewModel()
    {
    }

    public DeleteViewModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps) : base(user, hisApps)
    {
    }

    [FromRoute] [Required] public string AppId { get; set; }

    [Required] [FromRoute] [MaxLength(50)] public string SiteName { get; set; }

    public string AppName { get; set; }

    public void Recover(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps, string appName)
    {
        AppName = appName;
        RootRecover(user, hisApps);
    }
}