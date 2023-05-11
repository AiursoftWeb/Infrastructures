using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Developer.Models.AppsViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Developer.Models.SitesViewModels;

public class NewFolderViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public NewFolderViewModel()
    {
    }

    public NewFolderViewModel(DeveloperUser user) : base(user)
    {
    }

    public string NewFolderName { get; set; }

    [FromRoute] [Required] public string AppId { get; set; }

    [FromRoute] public string SiteName { get; set; }

    [FromRoute] public string Path { get; set; }

    public string AppName { get; set; }

    public void Recover(DeveloperUser user, string appName)
    {
        RootRecover(user);
        AppName = appName;
    }
}