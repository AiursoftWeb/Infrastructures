using Aiursoft.SDK.Models.Developer;
using System;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class IndexViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]

        public IndexViewModel() { }
        public IndexViewModel(DeveloperUser user) : base(user)
        {
        }
    }
}
