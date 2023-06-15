using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Portal.Models.AppsViewModels;

namespace Aiursoft.Portal.Models.SitesViewModels;

public class RenameFileViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public RenameFileViewModel()
    {
    }

    public RenameFileViewModel(PortalUser user) : base(user)
    {
    }

    [Required] public string AppId { get; set; }

    [Required] [MaxLength(50)] public string SiteName { get; set; }

    [Required] public string Path { get; set; }

    public string AppName { get; set; }

    [Required] public string NewName { get; set; }

    public void Recover(PortalUser user, string appName)
    {
        AppName = appName;
        RootRecover(user);
    }
}