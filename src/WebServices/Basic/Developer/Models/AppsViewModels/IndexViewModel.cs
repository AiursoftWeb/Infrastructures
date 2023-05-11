using System;

namespace Aiursoft.Developer.Models.AppsViewModels;

public class IndexViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public IndexViewModel()
    {
    }

    public IndexViewModel(DeveloperUser user) : base(user)
    {
    }
}