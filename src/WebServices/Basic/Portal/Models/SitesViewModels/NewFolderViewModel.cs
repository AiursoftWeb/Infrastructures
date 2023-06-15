using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Portal.Models.AppsViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Portal.Models.SitesViewModels;

public class NewFolderViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public NewFolderViewModel()
    {
    }

    public NewFolderViewModel(PortalUser user) : base(user)
    {
    }

    public string NewFolderName { get; set; }

    [FromRoute] [Required] public string AppId { get; set; }

    [FromRoute] public string SiteName { get; set; }

    [FromRoute] public string Path { get; set; }

    public string AppName { get; set; }

    public void Recover(PortalUser user, string appName)
    {
        RootRecover(user);
        AppName = appName;
    }
}