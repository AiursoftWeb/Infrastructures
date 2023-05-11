using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.AppsViewModels;

public class DeleteAppViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public DeleteAppViewModel()
    {
    }

    public DeleteAppViewModel(DeveloperUser user) : base(user)
    {
    }

    [Required] public virtual string AppId { get; set; }

    public virtual string AppName { get; set; }
}