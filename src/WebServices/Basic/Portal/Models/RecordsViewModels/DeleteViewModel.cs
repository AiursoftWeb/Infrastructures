using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Portal.Models.AppsViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Portal.Models.RecordsViewModels;

public class DeleteViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public DeleteViewModel()
    {
    }

    public DeleteViewModel(PortalUser user) : base(user)
    {
    }

    [FromRoute] [Required] public string AppId { get; set; }

    [Required] [FromRoute] [MaxLength(50)] public string RecordName { get; set; }

    public string AppName { get; set; }

    public void Recover(PortalUser user, string appName)
    {
        AppName = appName;
        RootRecover(user);
    }
}