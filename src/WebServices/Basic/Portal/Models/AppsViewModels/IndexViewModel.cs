using System;

namespace Aiursoft.Portal.Models.AppsViewModels;

public class IndexViewModel : AppLayoutModel
{
    [Obsolete("This method is only for framework", true)]
    public IndexViewModel()
    {
    }

    public IndexViewModel(PortalUser user) : base(user)
    {
    }
}