using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class DeleteAppViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public DeleteAppViewModel()
    {
    }

    public DeleteAppViewModel(PortalUser user) : base(user)
    {
    }

    [Required] public virtual string AppId { get; set; }

    public virtual string AppName { get; set; }
}