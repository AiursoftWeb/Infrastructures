using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class DeleteAppViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public DeleteAppViewModel()
    {
    }

    public DeleteAppViewModel(PortalUser user, IReadOnlyCollection<DirectoryApp> hisApps) : base(user, hisApps)
    {
    }

    [Required] public virtual string AppId { get; set; }

    public virtual string AppName { get; set; }
}