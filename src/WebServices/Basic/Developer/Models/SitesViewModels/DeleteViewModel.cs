using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Developer.Models.AppsViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Developer.Models.SitesViewModels;

public class DeleteViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public DeleteViewModel()
    {
    }

    public DeleteViewModel(DeveloperUser user) : base(user)
    {
    }

    [FromRoute] [Required] public string AppId { get; set; }

    [Required] [FromRoute] [MaxLength(50)] public string SiteName { get; set; }

    public string AppName { get; set; }

    public void Recover(DeveloperUser user, string appName)
    {
        AppName = appName;
        RootRecover(user);
    }
}